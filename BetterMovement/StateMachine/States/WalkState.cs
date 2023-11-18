using UnityEngine;
using UnityEngine.UI;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Walk")]
    public class WalkState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;
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
        private float _dashInput;
        private float _xInputInWholeNumber;
        private float _coyoteTimer;
        private float _accelRate;
        private float _runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
        private float _runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.



        public override void Init(PlayerController parent)
        {
            #region Get Components
            base.Init(parent);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_rb == null) _rb = parent.GetComponentInChildren<Rigidbody2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_anim == null) _anim = parent.PlayerAnimation;
            if (_data == null) _data = parent.PersistentPlayerData;
            if (_transition == null) _transition = parent.StateTransition;

            #endregion

            _coyoteTimer = 0;
            _jump = false;
            _data.jumpsLeft = _data.maxJumps;
            _dash = false;
            _dashInput = 0;
            

            Calculations();

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
            if (Input.GetButtonDown("Jump"))
                _jump = true;

        }

        public override void Update() {
            _anim.AdjustSpriteRotation(_xInput);
            CoyoteTimer();
        }

        public override void FixedUpdate() {
            _col.VerticalRaycasts(_cc, rayHeight);
            
            Move();
        }

        public override void ChangeState()
        {
       
            if (Mathf.Abs(_rb.velocity.x) <= 0.02 && Mathf.Abs(_xInput) < xInputTreshold)
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
                



        }


        public override void Exit() {

        }


        private void CoyoteTimer()
        {
            if (!_col.collisions.VerticalBottom)
                _coyoteTimer += Time.deltaTime;

        } // function

        public void Move()
        {
            float targetSpeed = _xInput * runMaxSpeed;
            float speedDif = targetSpeed - _rb.velocity.x;
            
            if (Mathf.Abs(targetSpeed) > 1f)
            {
                _anim.ChangeAnimationState(walkAnimation.name);
                _accelRate = _runAccelAmount;
            }
            else
            {
                _anim.ChangeAnimationState(slideAnimation.name);
                _accelRate = _runDeccelAmount;
            }

            float movement = speedDif * _accelRate;
            _rb.AddForce(movement * Vector2.right * _rb.mass, ForceMode2D.Force);
        }

        private void Calculations()
        {
            _runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
            _runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

            #region Variable Ranges
            runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
            runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
            #endregion
        }


    }
}