using UnityEngine;

namespace StateMachine
{
    public abstract class State<T> : ScriptableObject where T : MonoBehaviour
    {
        protected T _runner;

        public virtual void Init(T parent)
        {
            _runner = parent;
        }
        public abstract void CaptureInput();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void ChangeState();
        public abstract void Exit();

        public virtual void SetParameters(params object[] parameters)
        {
            // This can be overridden by specific states to handle parameters
        }

    }
}