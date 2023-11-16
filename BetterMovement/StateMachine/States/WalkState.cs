using UnityEngine;

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

        #endregion


        private float _xInput;
        private bool _jump;
        private float _dash;


        public float inputTreshold = .15f;
        private float _inputInWholeNumber;
        public float rayHeight = .1f;
        public bool visualizer = true;



        public float coyoteTime = .2f;
        private float coyoteTimer;





        public float runMaxSpeed = 8f; //Target speed we want the player to reach.
        public float runAcceleration = 8f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.
        


 

        string message;



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


            _data.jumpsLeft = _data.maxJumps;


            if(visualizer)
            {
                _sr.color = Color.green;
                Debug.Log("<color=green>Started a walk state</color>");

            }


            _anim.ChangeAnimationState("player-walk");
        }

        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");
            _inputInWholeNumber = (_xInput < inputTreshold) ? -1 : (_xInput > inputTreshold ? 1 : 0);
            _jump = Input.GetButtonDown("Jump");

            _dash = Input.GetAxisRaw("Dash");
        }

        public override void Update() {
            _col.VerticalRaycasts(_cc, rayHeight);
            _anim.AdjustSpriteRotation(_xInput);
            CoyoteTimer();
        }

        public override void FixedUpdate() {
            Move();
        }

        public override void ChangeState()
        {
            if (Mathf.Abs(_xInput) < inputTreshold && Mathf.Abs(_rb.velocity.x) > 0.1)
                _runner.SetState(typeof(SlideState));

            if (Mathf.Abs(_rb.velocity.x) <= 0.02)
                _runner.SetState(typeof(IdleState));

            if (coyoteTimer < coyoteTime && _jump)
                _runner.SetState(typeof(JumpState));

            if (!_col.collisions.VerticalBottom && !_jump)
                _runner.SetState(typeof(FallState));

            if (_dash > 0)
            {
                _runner.SetState(typeof(DashState));
                Debug.Log("pressed dahs");
            }
                
        }


        public override void Exit() {
            coyoteTimer = 0;
        }


        private void CoyoteTimer()
        {
            if (!_col.collisions.VerticalBottom)
                coyoteTimer += Time.deltaTime;

        } // function

        public void Move()
        {

            //Calculate the direction we want to move in and our desired velocity
            float targetSpeed = _inputInWholeNumber * runMaxSpeed;
            if (visualizer) Debug.Log("max speed is: " + targetSpeed);

            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - _rb.velocity.x;
            if (visualizer) Debug.Log("speed difference is: " + speedDif);

            //Calculate force along x-axis to apply to thr player
            float movement = speedDif * runAcceleration;
            if (visualizer) Debug.Log("movement is: " + movement);

            //Convert this to a vector and apply to rigidbody
            _rb.AddForce(movement * Vector2.right * _rb.mass, ForceMode2D.Force);
            //PhysicsUtilities.ApplyForceToReachVelocity(_rb, new Vector2(targetSpeed, _rb.velocity.y), 10000);
            if (visualizer) Debug.Log("Final speed is: " + _rb.velocity.x);
        }
    }
}