using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Fall")]
    public class FallState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        private float _xInput;
        private float _inputInWholeNumber;
        private bool _jump;
        public float inputTreshold = .15f;
        public bool assistedOverCorner = false;

        [SerializeField]
        private bool visualizer = true;


        [SerializeField]
        private float _verticalMovement = 50f;

        [SerializeField]
        private float _rayHeight = .1f;


        public bool _pressedJump = false;
        public float _jumpBufferTime = .4f;
        private float _jumpBufferTimer;





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

            #endregion

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
            _inputInWholeNumber = (_xInput < inputTreshold) ? -1 : (_xInput > inputTreshold ? 1 : 0);


            _jump = Input.GetButtonDown("Jump");

            //Debug.Log("why is this true_ " + _jump);



        }


        public override void Update()
        {
            _col.VerticalRaycasts(_cc, _rayHeight);

            if (_rb.velocity.y < -3f)
                _anim.ChangeAnimationState("player-air-falling");

            
            if (_jump || _pressedJump)
                _pressedJump = true;


            if (_pressedJump)
                JumpBufferTimer();



            AssistOverCorner();
            
        }

        public override void FixedUpdate()
        {

            if (Mathf.Abs(_xInput) > inputTreshold)
                _rb.AddForce(new Vector2(_data.dir * _verticalMovement, _rb.velocity.y));
        }


        public override void ChangeState()
        {


            if (_data.jumpsLeft >= 1 && _jump)
                _runner.SetState(typeof(JumpState));
            else if (_pressedJump && (_jumpBufferTimer < _jumpBufferTime && _col.collisions.VerticalBottom))
            {
                Debug.Log("Pressed before jump buffer timer is max");
                _jumpBufferTimer = 0;
                _pressedJump = false;
                _data.jumpsLeft = _data.maxJumps;
                _runner.SetState(typeof(JumpState));
            }
            else if (_col.collisions.VerticalBottom && Mathf.Abs(_xInput) > inputTreshold)
                _runner.SetState(typeof(WalkState));
            else if (_col.collisions.VerticalBottom && Mathf.Abs(_xInput) < inputTreshold && Mathf.Abs(_rb.velocity.x) > 0.1)
                _runner.SetState(typeof(SlideState));
            else if (_col.collisions.VerticalBottom)
                _runner.SetState(typeof(IdleState));

        }

        public override void Exit()
        {
            _rb.gravityScale = 2;
            
        }


        private void JumpBufferTimer()
        {
            _jumpBufferTimer += Time.deltaTime; // aloita ajan bufferointi

        }



        private void AssistOverCorner()
        {

        }
    }
}


