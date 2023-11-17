using UnityEngine;
using Utils.StateMachine;

namespace StateMachine
{
    public class PlayerController : StateRunner<PlayerController>
    {

        public PlayerAnimation PlayerAnimation;
        public PersistentPlayerData PersistentPlayerData;

        

        protected override void Awake()
        {

            PlayerAnimation = new PlayerAnimation(GetComponent<Animator>(), transform);
            base.Awake();
        }


    }
}