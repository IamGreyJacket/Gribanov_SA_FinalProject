using Racer.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Player 
{
    [CreateAssetMenu(fileName = "NewPlayerSave", menuName = "ScriptableObjects/PlayerSave", order = 1)]
    public class PlayerSave : ScriptableObject
    {
        public Dictionary<RaceType, bool> RaceAccess; //todo
        public bool TimeAttackIsOpen;
        public bool CircuitIsOpen;
        [Range(0, 1)]
        public float SoundVolume;
    }
}