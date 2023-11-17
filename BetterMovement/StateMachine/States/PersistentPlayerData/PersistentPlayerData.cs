using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [CreateAssetMenu(menuName = "States/PersitentPlayerData")]
    public class PersistentPlayerData : ScriptableObject
    {

        public int maxJumps = 2;
        public int jumpsLeft = 2;
        public float dashCooldown = .5f;
        public float baseGravityScale = 2f;


        [HideInInspector]
        public int dir = -1;
    }

}

