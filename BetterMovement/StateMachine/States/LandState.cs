using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Land")]
    public class LandState : PlayerStateWithMovement
    {
        #region Components

        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;

        #endregion

        private float _xInput;
        private bool _jump;
        public float inputTreshold = .15f;
        public float landingSlowdown = 0;

        [SerializeField]
        private float _rayHeight = .1f;

        [SerializeField]
        private bool visualizer = true;
        private bool _landingExacuted;
        private bool _isLandingFinished;
        public AnimationClip landAnimation;

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
            _col.VerticalRaycasts(_cc, _rayHeight);
            Land();
            
            Move(_xInput * moveMaxSpeed, moveMaxSpeed, moveAcceleration, moveDecceleration);

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


