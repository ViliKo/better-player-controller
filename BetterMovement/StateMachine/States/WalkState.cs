using UnityEngine;
using UnityEngine.UI;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Walk")]
    public class WalkState : PlayerStateWithMovement
    {
        #region Components

        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private Text _transition;
        #endregion


        [Header("Movement State settings")]
        public float xInputTreshold = .15f;
        public float dashInputTreshold = .15f;
        public float rayHeight = .1f;
        public bool visualizer = true;
        public float coyoteTime = .2f;
        public AnimationClip walkAnimation;
        public AnimationClip slideAnimation;
        public float runMaxSpeed = 8f; //Target speed we want the player to reach.
        public float runAcceleration = 8f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
        public float runDecceleration = 0.5f; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.



        private float _xInput;
        private bool _jump;
        private bool _dash;
        private float _coyoteTimer;

        private float selectedInputTreshold = .15f;
        private bool _spiritState;



        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            #region Get Components
            base.Init(parent, characterMode);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_data == null) _data = parent.PersistentPlayerData;
            if (_transition == null) _transition = parent.StateTransition;

            #endregion

            _coyoteTimer = 0;
            _jump = false;
            _data.jumpsLeft = _data.maxJumps;
            _dash = false;
            _spiritState = false;

 
            if (visualizer)
                _sr.color = Color.green;
            
        }

        public override void CaptureInput()
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > xInputTreshold)
            {
                _xInput = Input.GetAxis("Horizontal");
            } else
            {
                _xInput = 0;
            }


            if (Input.GetAxisRaw("Dash") > dashInputTreshold)
            {
                _dash = true;
            }

    
            if (Input.GetAxisRaw("Select") > selectedInputTreshold && Input.GetKey(KeyCode.Joystick1Button3))
            {
                _spiritState = true;
            }

            if (Input.GetButtonDown("Jump"))
                _jump = true;

        }

        public override void Update() {
            _anim.AdjustSpriteRotation(_xInput);
            CoyoteTimer();
        }

        public override void FixedUpdate() {
            _col.VerticalRaycasts(_cc, rayHeight);

            Move(_xInput * runMaxSpeed, runMaxSpeed, runAcceleration, runDecceleration, walkAnimation.name, slideAnimation.name);
        }




        public override void ChangeState()
        {
       
            if (Mathf.Abs(_rb.velocity.x) <= 0.3)
            {
                _transition.text = "Kavely -> horisonttaalinen nopeus oli vahemman kuin 0.02 -> Lepo";
                _runner.SetState(typeof(IdleState));
            }
                

            if (_coyoteTimer < coyoteTime && _jump)
            {
                _transition.text = "Kavely -> coyote ajastin oli pienempi kuin maaritetty aika ja hyppya on painettu -> Hyppy";
                _runner.SetState(typeof(JumpState));
            }
                
            
                

            if (!_col.collisions.VerticalBottom && !_jump)
            {
                _transition.text = "Kavely -> maahan osoittava raycast ei osunut ja ei ole painanut hyppya -> Putoaminen";
                _runner.SetState(typeof(FallState));
            }


            if (_dash)
            {
                _transition.text = "Kavely -> dash nappia painettu -> Dash";
                _dash = false;
                _runner.ActivateAbility(typeof(DashState), _data.dashCooldown);
                
            }

            if (_spiritState)
            {
                _runner.ActivateAbility(typeof(SpiritModeEnterState), 10f);
            }
                



        }


        public override void Exit() {

        }


        private void CoyoteTimer()
        {
            if (!_col.collisions.VerticalBottom)
                _coyoteTimer += Time.deltaTime;

        } // function



    }
}