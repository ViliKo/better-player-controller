using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{

    [CreateAssetMenu(menuName = "States/Player/WallJump")]
    public class WallJumpState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        [Header("Wall jump settings")]
        public float wallJumpAngle = 25f;
        public float baseWallJumpSpeed = 60f;
        public float extraWallJumpSpeed = 20f;
        Vector2 wallJumpSpeedVector;
        public bool visualizer = false;

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
            WallJump(); // Lasken seinahypyssa tuon kulman saa naha onko tuo hyva ratkaisu
        }

        public override void CaptureInput(){} // TODO: needs an implimentation of a double jump

        public override void Update() {}

        public override void FixedUpdate() {}

        public override void ChangeState()
        {
            if (_rb.velocity.y < 0)
                _runner.SetState(typeof(FallState));
        }

        public override void Exit(){}

        private void WallJump()
        {
            float wallJumpSpeedXvector;
            float wallJumpSpeedYvector;

            wallJumpSpeedYvector = extraWallJumpSpeed + baseWallJumpSpeed;
            wallJumpSpeedXvector = wallJumpSpeedYvector * Mathf.Tan(Mathf.Deg2Rad * wallJumpAngle) * _sr.transform.localScale.x;


            wallJumpSpeedVector = new Vector2(wallJumpSpeedXvector, wallJumpSpeedYvector + _rb.velocity.y);

            _rb.AddForce(wallJumpSpeedVector, ForceMode2D.Impulse);
            _anim.AdjustSpriteRotation(_sr.transform.localScale.x);

            if (visualizer)
                Debug.Log("Im on wall jump state");
        }
    }

   
}

