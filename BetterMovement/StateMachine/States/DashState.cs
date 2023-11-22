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
        public float rayHeight = .1f;



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
            _initialPositionX = _rb.position.x;
            _isDashing = true;

            _rb.velocity = Vector2.zero;


        }


        public override void CaptureInput()
        {
 
        }

        public override void Update()
        {
            

            _col.HorizontalRaycasts(-_sr.transform.localScale.x, _cc, dashDistance, false, true); // raycastaa suuntaan mihin dashaa
            _col.VerticalRaycasts(_cc, rayHeight);

            float newDashDistance;

            if (_col.collisions.HorizontalBottomUp) // jos on osunut collideriin
            {
                newDashDistance = _col.hitDistance; // niin dashin etaisyys on colliderin etaisyys viela pienennettyna

                if (newDashDistance < 1)  // jos colliderin etaisyys on vahempi kuin kaksi ala sitten dashaa
                    _isDashing = false;

            }
            else
                newDashDistance = dashDistance;  // muuten dashaus on normaali






            if (_isDashing)
            {
                float distanceTraveled = Mathf.Abs(_rb.position.x - _initialPositionX);

                if (distanceTraveled < newDashDistance)
                {
                    _rb.AddForce(new Vector2(dashForce * -_sr.transform.localScale.x * _rb.mass, 0f), ForceMode2D.Impulse);
                    _anim.ChangeAnimationState("player-dash");
                }  
                else
                    StopDash();
            }

        }

        public override void FixedUpdate()
        {
        }

        public override void ChangeState()
        {
            if (_isDashing) return;

            if (_col.collisions.VerticalBottom)
                _runner.SetState(typeof(IdleState));
            else
                _runner.SetState(typeof(FallState));
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
