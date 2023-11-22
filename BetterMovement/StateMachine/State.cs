using UnityEngine;

namespace StateMachine
{
    public abstract class State<T> : ScriptableObject where T : MonoBehaviour
    {
        protected T _runner;
        protected CharacterMode _currentMode;
        protected Vector2 _startPosition;

        public virtual void Init(T parent, CharacterMode currentMode = CharacterMode.Normal)
        {
            _runner = parent;
            _currentMode = currentMode; 
            _startPosition = _runner.transform.position;
        }
        public abstract void CaptureInput();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void ChangeState();
        public abstract void Exit();

        public virtual void SetParameters(params object[] parameters)
        {
            // Tama voidaan ylikirjoittaa tiettyyn tilaan mita parametreja tarvitaankin
        }

    }
}