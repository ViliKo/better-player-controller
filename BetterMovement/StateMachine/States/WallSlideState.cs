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

        private float _yInput;
        private bool _jump;
        public float inputTreshold = .15f;

        [SerializeField]
        private bool visualizer = true;
        private bool _isWallSliding = false;


        public float slideSpeed = 2f;


        [SerializeField]
        private float _rayHeight = .1f;






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


            _isWallSliding = true;

            if (visualizer)
                _sr.color = Color.magenta;
        }

        public override void CaptureInput()
        {
            _yInput = Input.GetAxis("Vertical");
            _jump = Input.GetButtonDown("Jump");




        }




        public override void Update()
        {

            
            





        }

        public override void FixedUpdate()
        {
            _col.VerticalRaycasts(_cc, _rayHeight);

            _rb.velocity = new Vector2(0, -slideSpeed);
        }


        public override void ChangeState()
        {
            if (_yInput > inputTreshold)
                _runner.SetState(typeof(WallClimbState));

          

        }

        public override void Exit()
        {
 

        }
    }
}


