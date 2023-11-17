using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Land")]
    public class LandState : State<PlayerController>
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


        public float inputTreshold = .15f;
        private float _inputInWholeNumber;
        public bool visualizer = true;

        public AnimationClip landAnimation;

        public float runMaxSpeed = 8f; //Target speed we want the player to reach.
        public float runAcceleration = 8f; //Time (approx.) time we want it to take for the player to accelerate from 0 to the runMaxSpeed.









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

            _anim.ChangeAnimationState(landAnimation.name);

            _rb.velocity = Vector2.zero;

            _data.jumpsLeft = _data.maxJumps;


            if (visualizer)
            {
                _sr.color = Color.green;
                Debug.Log("<color=green>Started a land state</color>");

            }

            
        }

        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");
            _inputInWholeNumber = (_xInput < inputTreshold) ? -1 : (_xInput > inputTreshold ? 1 : 0);

        }

        public override void Update()
        {
            Debug.Log("Is animation finished: " + _anim.isAnimationFinished());
            _anim.AdjustSpriteRotation(_xInput);


        }

        public override void FixedUpdate()
        {
            Move();
        }

        public override void ChangeState()
        {

            if (_anim.isAnimationFinished())
            {
                
                if (Mathf.Abs(_xInput) > inputTreshold)
                    _runner.SetState(typeof(WalkState));
                if (Mathf.Abs(_xInput) < inputTreshold && Mathf.Abs(_rb.velocity.x) > 0.1)
                    _runner.SetState(typeof(SlideState));
                if (Mathf.Abs(_rb.velocity.x) <= 0.02)
                    _runner.SetState(typeof(IdleState));
            }
        }


        public override void Exit()
        {

        }




        public void Move()
        {


            _rb.AddForce(_inputInWholeNumber * Vector2.right * _rb.mass, ForceMode2D.Force);

        }
    }
}