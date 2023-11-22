using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/LedgeClimb")]


    public class LedgeClimbState : State<PlayerController>
    {
        #region Components

        private Rigidbody2D _rb;
        private PlatformerController2D _col;
        private PersistentPlayerData _data;
        private CapsuleCollider2D _cc;
        private SpriteRenderer _sr;
        private PlayerAnimation _anim;


        #endregion

        [Header("Ledge Climb Settings")]
        public Vector2 offset1;
        public Vector2 offset2;
        public AnimationClip ledgeClimbAnimation;

        private bool isClimbingCorner;
        private Vector2 climbBegunPosition;
        private Vector2 climbOverPosition;


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

            _col.Reset();
            isClimbingCorner = true;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0;

            Vector2 ledgePosition = _rb.transform.position;
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = new Vector2(ledgePosition.x + (offset2.x * -_sr.transform.localScale.x), ledgePosition.y + offset2.y);

            _rb.transform.position = climbBegunPosition;
            _anim.ChangeAnimationState(ledgeClimbAnimation.name); // Tässä animaatiossa on tapahtuma, joka laukaisee alla olevan funktion


        }


        public override void CaptureInput()
        {
            
        }



        public override void Update()
        {
            if (_anim.getCurrentAnimationName(ledgeClimbAnimation.name) && _anim.isAnimationFinished())
            {
                _rb.transform.position = climbOverPosition;
                _rb.gravityScale = _data.baseGravityScale;
                isClimbingCorner = false;
            }
        }

        public override void FixedUpdate()
        {
            
        }

        public override void ChangeState()
        {
            if (!isClimbingCorner)
                _runner.SetState(typeof(IdleState));
        }

        public override void Exit()
        {
           
        }



     

    }

}

