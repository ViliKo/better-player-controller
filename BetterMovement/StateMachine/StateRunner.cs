using StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.StateMachine
{
    [Serializable]

    public abstract class StateRunner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        private List<State<T>> _states;
        private State<T> _activeState;

        private CooldownManager _cooldownManager;

        protected virtual void Awake()
        {
            _cooldownManager = new CooldownManager(); // New line

            SetState(_states[0].GetType());
        }

        public void SetState(Type newStateType)
        {
            if (_activeState != null)
                _activeState.Exit();
            

            _activeState = _states.First(s => s.GetType() == newStateType);
            _activeState.Init(GetComponent<T>());
        }

        public void ActivateAbility(Type abilityType, float cooldownTime)
        {
            if (!_cooldownManager.IsAbilityOnCooldown(abilityType))
            {
                SetState(abilityType);
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
            _cooldownManager.UpdateCooldowns(); // New line
            _activeState.ChangeState();


        }

        private void FixedUpdate()
        {
            _activeState.FixedUpdate();
        }

      
    }
}