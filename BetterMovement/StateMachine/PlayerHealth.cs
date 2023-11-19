using UnityEngine;

namespace StateMachine
{
    public class PlayerHealth : MonoBehaviour
    {
        public PlayerController playerController;

        private void OnTriggerEnter2D(Collider2D collision)
        {

            
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("I have collided");
                int damageAmount = 10;
                playerController.SetState(typeof(OnHurtState), damageAmount);
            }
        }

    }
}