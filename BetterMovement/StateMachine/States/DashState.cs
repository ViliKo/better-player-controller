using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/Dash")]
    public class DashState : State<PlayerController>
    {

        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        public float dashDistance = 5f; // Desired distance to dash
        public float dashForce = 10f; // Speed of the dash
        private bool _isDashing;
        private float _initialPositionX;

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
            _initialPositionX = _rb.position.x;
            _isDashing = true;
        }


        public override void CaptureInput()
        {
 
        }

        public override void Update()
        {

            if (_isDashing)
            {
                float distanceTraveled = Mathf.Abs(_rb.position.x - _initialPositionX);

                if (distanceTraveled < dashDistance)
                    _rb.AddForce(new Vector2(dashForce * -_sr.transform.localScale.x, 0f), ForceMode2D.Impulse);
                else
                    StopDash();
            }

        }

        public override void FixedUpdate()
        {
        }

        public override void ChangeState()
        {
            if (!_isDashing)
                _runner.SetState(typeof(IdleState));
        }

        public override void Exit()
        {
        }

        void StopDash()
        {
            _isDashing = false;
            _rb.velocity = Vector2.zero;
        }
    }

}
