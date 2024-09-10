using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.AI
{
    public class BotCheckpoint : MonoBehaviour
    {
        [SerializeField, Tooltip("Target speed for bot to maintain (the value is Km/H)")]
        private float _targetSpeed;
        public float TargetSpeed => _targetSpeed;

        private void OnTriggerEnter(Collider other)
        {
            var bot = other.GetComponentInParent<BotCarController>();
            if(bot == null)
            {
                Debug.LogWarning($"Bot component is not found on {other.name}");
                return;
            }
            bot.NextCheckpoint(this);
            //
        }
    }
}