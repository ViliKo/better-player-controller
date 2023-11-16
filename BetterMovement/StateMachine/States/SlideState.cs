
using UnityEngine;

namespace StateMachine
{

    [CreateAssetMenu(menuName = "States/Player/Slide")]
    public class SlideState : State<PlayerController>
    {

        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        public bool visualizer = true;


        private float _xInput;
        private bool _jump;
        public float inputTreshold = .3f;

        public float baseDrag = .5f;
        public float newDrag = 4f;
        public float rayHeight = .1f;

        

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


            _data.jumpsLeft = _data.maxJumps; // resettaa hypyt, jos menee tahan tilaan
            _rb.drag = newDrag; // uusi dragi hidastaa pelaajaa

            _anim.ChangeAnimationState("player-slide");


            if(visualizer)
            {
                Debug.Log("<color=red>Started a slide state</color>");
                _sr.color = Color.red;
            }



        }

        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");
            _jump = Input.GetButtonDown("Jump");
        }


        public override void FixedUpdate()
        {
            
        }

        public override void Update()
        {
            _col.VerticalRaycasts(_cc, rayHeight);

        }

        public override void ChangeState()
        {
            if (Mathf.Abs(_xInput) > inputTreshold)
                _runner.SetState(typeof(WalkState));

            if (Mathf.Abs(_rb.velocity.x) < .1f)
                _runner.SetState(typeof(IdleState));

            if (_jump && _col.collisions.VerticalBottom)
                _runner.SetState(typeof(JumpState));

            if (!_col.collisions.VerticalBottom && !_jump)
                _runner.SetState(typeof(FallState));

        }

        public override void Exit()
        {
            _rb.drag = baseDrag; // lahdossa laita takaisin alkuperainen raahaus
        }
    }
}

