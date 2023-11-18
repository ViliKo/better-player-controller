
using UnityEngine;

namespace StateMachine
{

    public class PlayerAnimation
    {
        private string _currentState;
        private readonly Animator _anim;
        private readonly Transform _transform;

        public PlayerAnimation(Animator animator, Transform transform)
        {
            _anim = animator;
            _transform = transform;
        }

        public void ChangeAnimationState(string newState)
        {
            if (_currentState == newState) return;

            _anim.Play(newState);
            _currentState = newState;
        }

        public void AdjustSpriteRotation(float xInput)
        {
            if (xInput != 0)
            {
                _transform.localScale = new Vector3(-Mathf.Sign(xInput), 1, 1);
            }
        }

        public bool isAnimationFinished() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;

        public bool getCurrentAnimationName(string isAnimation) => _anim.GetCurrentAnimatorStateInfo(0).IsName(isAnimation);


    }
}

