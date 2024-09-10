using Racer.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Player 
{
    [CreateAssetMenu(fileName = "NewPlayerSave", menuName = "ScriptableObjects/PlayerSave", order = 1)]
    public class PlayerSaveSO : ScriptableObject
    {
        public PlayerSaveInfo Info = new PlayerSaveInfo()
        {
            TimeAttackIsOpen = false,
            CircuitIsOpen = false,
            CarID = 0,
            TuningIsOpen = false,
            WheelsID = 0,
            WheelsScale = new Vector3(100, 100, 100),
            SpoilerID = 0,
            SoundVolume = 1f
        };
    }

    [System.Serializable]
    public struct PlayerSaveInfo
    {
        public bool TimeAttackIsOpen;
        public bool CircuitIsOpen;
        public int CarID;
        public bool TuningIsOpen;
        public int WheelsID;
        public Vector3 WheelsScale;
        public int SpoilerID;
        [Range(0, 1)]
        public float SoundVolume;
    }
}