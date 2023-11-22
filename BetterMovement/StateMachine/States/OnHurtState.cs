using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/Player/OnHurt")]
    public class OnHurtState : State<PlayerController>
    {
        private float hurtDuration = 2.0f;
        private float hurtTimer;
        private int damageTaken;

        public override void Init(PlayerController parent, CharacterMode characterMode)
        {
            base.Init(parent, characterMode);
        }

        public override void SetParameters(params object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is int)
            {
                damageTaken = (int)parameters[0];
            }

            hurtTimer = hurtDuration;
            Debug.Log($"Player took {damageTaken} damage!");
        }



        public override void Update()
        {
            hurtTimer -= Time.deltaTime;


        }

        public override void Exit()
        {
 
        }

        public override void CaptureInput()
        {

        }

        public override void FixedUpdate()
        {

        }

        public override void ChangeState()
        {
            if (hurtTimer <= 0)
            {
                _runner.SetState(typeof(IdleState));
            }
        }
    }
}