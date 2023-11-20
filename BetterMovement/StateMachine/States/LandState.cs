using System.Collections;
using System.Collections.Generic;
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
        private bool _jump;
        public float inputTreshold = .15f;
        public float landingSlowdown = 0;

        [SerializeField]
        private bool visualizer = true;
        private bool _landingExacuted;
        private bool _isLandingFinished;
        public AnimationClip landAnimation;


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

            _jump = false;
            _rb.gravityScale = 2;
            _landingExacuted = false;
            _isLandingFinished = false;

            if (visualizer)
                _sr.color = Color.magenta;
        }

        public override void CaptureInput()
        {
            _xInput = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump"))
                _jump = true;
        }


        public override void Update()
        {
            _anim.AdjustSpriteRotation(_xInput);

            if (_landingExacuted && _anim.getCurrentAnimationName(landAnimation.name))
                _isLandingFinished = _anim.isAnimationFinished();
        }

        public override void FixedUpdate()
        {
            Land();
        }

        private void Land()
        {
            if (_col.collisions.VerticalBottom && !_landingExacuted)
            {
                _anim.ChangeAnimationState(landAnimation.name);
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
                _landingExacuted = true;
            }
        }


        public override void ChangeState()
        {
            if (_isLandingFinished)
            {
                _landingExacuted = false;
                _isLandingFinished = false;
                _data.jumpsLeft = _data.maxJumps;

                if (_jump)
                    _runner.SetState(typeof(JumpState));
                else if (Mathf.Abs(_xInput) > inputTreshold)
                    _runner.SetState(typeof(WalkState));
                else
                    _runner.SetState(typeof(IdleState));
            }
            else if (!_col.collisions.VerticalBottom && _rb.velocity.y < 0)
                _runner.SetState(typeof(FallState));
           
        }

        public override void Exit()
        {
            _landingExacuted = false;
            _isLandingFinished = false;
        }
    }
}


