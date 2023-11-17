using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{

    [CreateAssetMenu(menuName = "States/Player/WallClimb")]
    public class WallClimbState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;

        #endregion

        private float _yInput;
        public float climbSpeed = 3f;
        public float inputTreshold = .15f;

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

            _rb.gravityScale = 0;
            Debug.Log("Im on wall climb state");
        }



        public override void CaptureInput()
        {
            _yInput = Input.GetAxis("Vertical");
        }

        public override void ChangeState()
        {
            if (_yInput < inputTreshold)
                _runner.SetState(typeof(WallSlideState));
        }

        public override void Exit()
        {
            _rb.gravityScale = _data.baseGravityScale;
        }

        public override void FixedUpdate()
        {
            WallClimb();

        }

        public override void Update()
        {
           
        }

        private void WallClimb()
        {

            // Get vertical input for climbing up or down
            

            // Calculate the climb direction
            Vector2 climbDirection = new Vector2(0, _yInput).normalized;

            // Apply the climb direction multiplied by the climb speed
            _rb.velocity = new Vector2(0, climbDirection.y * climbSpeed);

 

        }


    }

}

