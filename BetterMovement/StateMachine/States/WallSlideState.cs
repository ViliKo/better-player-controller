using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/WallSlide")]
    public class WallSlideState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        [Header("Wall Slide Settings")]
        public float slideSpeed = 2f;
        public float rayHeight = .1f;
        public AnimationClip wallSlideAnimation;
        public bool visualizer = true;

        [SerializeField]
        private float _rayHeight = .1f;
        private float _yInput;
        private bool _jump;
        public float inputTreshold = .15f;





        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            #region Get Components
            base.Init(parent, characterMode);
            if (_col == null) _col = parent.GetComponentInChildren<PlatformerController2D>();
            if (_cc == null) _cc = parent.GetComponentInChildren<CapsuleCollider2D>();
            if (_rb == null) _rb = parent.GetComponentInChildren<Rigidbody2D>();
            if (_sr == null) _sr = parent.GetComponentInChildren<SpriteRenderer>();
            if (_anim == null) _anim = parent.PlayerAnimation;
            if (_data == null) _data = parent.PersistentPlayerData;

            #endregion

            _anim.ChangeAnimationState(wallSlideAnimation.name);
            _data.jumpsLeft = _data.maxJumps;

            if (visualizer)
                _sr.color = Color.magenta;
        }

        public override void CaptureInput()
        {
            _yInput = Input.GetAxis("Vertical");
            _jump = Input.GetButtonDown("Jump");
        }




        public override void Update() { }

        public override void FixedUpdate()
        {
            _col.VerticalRaycasts(_cc, _rayHeight);
            _col.HorizontalRaycasts(-_sr.transform.localScale.x, _cc, .1f, false, false, true, true);
            _rb.velocity = new Vector2(0, -slideSpeed);
        }


        public override void ChangeState()
        {
            if (_yInput > inputTreshold)
                _runner.SetState(typeof(WallClimbState));
            else if (_col.collisions.VerticalBottom) 
                _runner.SetState(typeof(IdleState));
            else if (_jump)
                _runner.SetState(typeof(JumpState));
            else if (!_col.collisions.HorizontalUp && !_col.collisions.HorizontalUpLower)
            {
                _col.Reset();
                _data.jumpsLeft -= 1;
                _runner.SetState(typeof(FallState));
            }
        }

        public override void Exit() {}
    }
}


