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

        [Header("Wall Climb settings")]
        public float climbSpeed = 3f;
        public float inputTreshold = .15f;
        public AnimationClip WallClimbAnimation;
        public bool visualizer = false;


        // yksityiset muuttujat
        private float _yInput;
        private bool _jump;



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

            _jump = false;

            _rb.gravityScale = 0; // seinakiipeillyssa pitaa ottaa gravitaatio pois
            _data.jumpsLeft = _data.maxJumps;
            

            if (visualizer)
                Debug.Log("Im on wall climb state");
        }



        public override void CaptureInput()
        {
            _yInput = Input.GetAxis("Vertical");
            _jump = Input.GetButtonDown("Jump");
        }


        public override void Update() {}

        public override void FixedUpdate() {

            _col.HorizontalRaycasts(-_sr.transform.localScale.x, _cc, .1f, false, false, true, true);
            WallClimb();
        }

        
        public override void ChangeState()
        {
            if (_rb.velocity.y < 0 || _yInput < inputTreshold) // jos inputti on pienempi kuin treshold ala kiipeaa
                _runner.SetState(typeof(WallSlideState));
            if (_jump)  // jos painat hyppya mene seina hyppyyn 
                _runner.SetState(typeof(JumpState));
            if (!_col.collisions.HorizontalUp && _col.collisions.HorizontalUpLower)
                _runner.SetState(typeof(LedgeClimbState));


        }

        public override void Exit()
        {
            _rb.gravityScale = _data.baseGravityScale; // pois lahdossa laita gravitaatio takaisin
        }



        private void WallClimb()
        {
            // Ota kiipeamisen suunta jos se on isompi kuin imput treshold sitten kiipea
            float positiveYInput = (_yInput > inputTreshold) ? 1 : 0;

            if (!(positiveYInput == 0))  // laita se rigidbodyn nopeuteen
            {
                _rb.velocity = new Vector2(0, positiveYInput * climbSpeed);
                _anim.ChangeAnimationState(WallClimbAnimation.name);
            }
                
        }


    }

}

