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


        [HideInInspector]
        public int dir = -1;
    }

}

