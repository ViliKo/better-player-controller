using UnityEngine;

namespace StateMachine
{
    public abstract class PlayerStateWithMovement : State<PlayerController>
    {
        float _accelRate;



        protected void Move(float targetSpeed, float runAccelAmount, float runDeccelAmount, Rigidbody2D _rb, string anim1, string anim2, PlayerAnimation _anim = null)
        {

            float speedDif = targetSpeed - _rb.velocity.x;

            if (Mathf.Abs(targetSpeed) > 1f)
            {
                _anim.ChangeAnimationState(anim1);
                _accelRate = runAccelAmount;
            }
            else
            {
                _anim.ChangeAnimationState(anim2);
                _accelRate = runDeccelAmount;
            }

            float movement = speedDif * _accelRate;
            _rb.AddForce(movement * Vector2.right * _rb.mass, ForceMode2D.Force);
        }
    }
}
