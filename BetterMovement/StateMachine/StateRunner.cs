using StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.StateMachine
{


    public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private List<State<T>> _states;
        private State<T> _activeState;
        private CharacterMode _currentMode = CharacterMode.Normal;
        private CooldownManager _cooldownManager;
        protected Vector2 _startPosition;


        private float _spiritModeTimer = 0f;
        private float _spiritModeDuration = 3f; // Adjust the duration as needed

        protected virtual void Awake()
        {
            _cooldownManager = new CooldownManager(); // New line
            SetState(_states[0].GetType());
        }

        public void SetMode(CharacterMode mode, Vector2 _startPosition)
        {
            _currentMode = mode;
            transform.position = _startPosition;
            // You might want to handle any specific logic for mode change here
        }

        public void SetState(Type newStateType, params object[] parameters)
        {
            if (_activeState != null)
                _activeState.Exit();
            

            _activeState = _states.First(s => s.GetType() == newStateType);
            _activeState.Init(GetComponent<T>(), _currentMode);

            // Laita parametrit jos tila tukee niita
            _activeState.SetParameters(parameters);
        }

        public void ActivateAbility(Type abilityType, float cooldownTime, params object[] parameters)
        {
            if (!_cooldownManager.IsAbilityOnCooldown(abilityType))
            {
                if (abilityType == typeof(SpiritModeEnterState))
                {
                    SetMode(CharacterMode.Spirit, transform.position);
                    _spiritModeTimer = _spiritModeDuration;
                    _startPosition = transform.position;
                }

                SetState(abilityType, parameters);
                _cooldownManager.StartCooldown(abilityType, cooldownTime);
            }
            else
            {
                Debug.Log($"{abilityType.Name} is on cooldown.");
            }
        }



        private void Update()
        {
            _activeState.CaptureInput();
            _activeState.Update();
            _cooldownManager.UpdateCooldowns();

            if (_currentMode == CharacterMode.Spirit)
            {
                _spiritModeTimer -= Time.deltaTime;
                if (_spiritModeTimer <= 0)
                {
                    SetMode(CharacterMode.Normal, _startPosition);
                    // Additional logic for transitioning back to the normal mode
                    Destroy(GameObject.FindGameObjectWithTag("freezeFrame"));
                }
            }


            _activeState.ChangeState();


        }

        private void FixedUpdate()
        {
            _activeState.FixedUpdate();
        }

      
    }
}