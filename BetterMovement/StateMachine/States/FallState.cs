using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Fall")]
    public class FallState : PlayerStateWithMovement
    {
        #region Components

        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;

        #endregion

        private float _xInput;
        private bool _jump;
        private float _dash;
        public float inputTreshold = .15f;
        public bool assistedOverCorner = false;

        [SerializeField]
        private bool visualizer = true;


        [SerializeField]
        private float _rayHeight = .1f;

        public bool _pressedJump = false;
        public float _jumpBufferTime = .4f;
        private float _jumpBufferTimer;

        public float moveMaxSpeed = 8f; //Target speed we want the player to reach. asd
        public float moveAcceleration = 8f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
        public float moveDecceleration = 0.5f; //Time (approx.) we want it to take for the player to accelerate from runMaxSpeed to 0.



        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            #region Get Components
            base.Init(parent, characterMode);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_data == null) _data = parent.PersistentPlayerData;

            #endregion

            _col.Reset();

            _rb.gravityScale = 2;
            _pressedJump = false;


            _jumpBufferTimer = 0;
            _rb.gravityScale = 5;
            _anim.ChangeAnimationState("player-air-peak");

            if (visualizer)
                _sr.color = Color.magenta;
        }
  
        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");
            _dash = Input.GetAxisRaw("Dash");
            _jump = Input.GetButtonDown("Jump");
        }


        public override void Update()
        {
            if (_rb.velocity.y < -3f)
                _anim.ChangeAnimationState("player-air-falling");

            
            if (_jump || _pressedJump)
                _pressedJump = true;


            if (_pressedJump)
                JumpBufferTimer();

            _anim.AdjustSpriteRotation(_xInput);
        }

        public override void FixedUpdate()
        {
            _col.VerticalRaycasts(_cc, _rayHeight);
            _col.HorizontalRaycasts(-_sr.transform.localScale.x, _cc, .1f, false, false, true, true);
            Move(_xInput * moveMaxSpeed, moveMaxSpeed, moveAcceleration, moveDecceleration);

            AssistOverCorner();
        }

        public override void ChangeState()
        {


            if (_data.jumpsLeft >= 1 && _jump)
                _runner.SetState(typeof(JumpState));
            else if (_pressedJump && (_jumpBufferTimer < _jumpBufferTime && _col.collisions.VerticalBottom))
            {
                _jumpBufferTimer = 0;
                _pressedJump = false;
                _data.jumpsLeft = _data.maxJumps;
                _runner.SetState(typeof(JumpState));
            }
            else if(_col.collisions.VerticalBottom)
            {
                _runner.SetState(typeof(LandState));
            }

            if (_dash > 0)
                _runner.ActivateAbility(typeof(DashState), _data.dashCooldown);

            if (_col.collisions.HorizontalUp && _col.collisions.HorizontalUpLower){
                _col.collisions.HorizontalBottomUp = false;
                _col.collisions.HorizontalUpLower = false;
                _runner.SetState(typeof(WallSlideState));
            }


        }

        public override void Exit()
        {

        }


        private void JumpBufferTimer()
        {
            _jumpBufferTimer += Time.deltaTime; // aloita ajan bufferointi

        }



        private void AssistOverCorner()
        {
            // TODO: assis over corner function
        }

        private void Move()
        {

        }
    }
}


