using UnityEngine;
using UnityEngine.UI;
using Utils.StateMachine;

namespace StateMachine
{
    public class PlayerController : StateRunner<PlayerController>
    {

        public PlayerAnimation PlayerAnimation;
        public AudioSource PlayerSound;
        public PersistentPlayerData PersistentPlayerData;
        public Text StateTransition;



        protected override void Awake()
        {

            PlayerAnimation = new PlayerAnimation(GetComponent<Animator>(), transform);
            base.Awake();
        }


    }
}