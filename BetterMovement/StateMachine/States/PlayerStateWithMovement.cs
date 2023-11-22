using UnityEngine;

namespace StateMachine
{
    public abstract class PlayerStateWithMovement : State<PlayerController>
    {
        protected Rigidbody2D _rb;
        protected PlayerAnimation _anim;


        float _accelRate;

        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            base.Init(parent, characterMode);

            _rb = parent.GetComponentInChildren<Rigidbody2D>();
            _anim = parent.PlayerAnimation;
        }

        protected float Accelerate(float targetSpeed, float currentSpeed, float acceleration, float maxSpeed)
        {
            float speedDiff = targetSpeed - currentSpeed;
            float accelRate = (50 * acceleration) / maxSpeed;
            float movement = speedDiff * accelRate;
            return movement;
        }


        protected void Move(float targetSpeed, float moveMaxSpeed, float moveAccelAmount, float moveDeccelAmount, string anim1 = null, string anim2 = null)
        {

            
            
            if (Mathf.Abs(targetSpeed) > 1f)
            {
                if (anim1 != null)
                    _anim.ChangeAnimationState(anim1);
                _accelRate = moveAccelAmount;
                
            }
            else
            {
                if (anim1 != null)
                    _anim.ChangeAnimationState(anim2);
                _accelRate = moveDeccelAmount;
            }

            float movement = Accelerate(targetSpeed, _rb.velocity.x, _accelRate, moveMaxSpeed);
            _rb.AddForce(movement * Vector2.right * _rb.mass, ForceMode2D.Force);
        }
    }
}
