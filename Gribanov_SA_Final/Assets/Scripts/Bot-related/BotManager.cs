﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.AI
{
    public class BotManager : MonoBehaviour
    {
        [Header("Bot-related"), SerializeField]
        private BotCheckpoint[] _botCheckpoints;

        private BotCarController[] _bots;

        private void Awake()
        {
            _bots = FindObjectsOfType<BotCarController>();
            foreach (var bot in _bots)
            {
                bot.SetCheckpoints(_botCheckpoints);
            }
        }

        private void Start()
        {

        }
    }
}