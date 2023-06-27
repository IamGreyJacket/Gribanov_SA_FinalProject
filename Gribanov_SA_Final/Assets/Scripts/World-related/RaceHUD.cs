using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Racer.Managers.Assistants;
using System;

namespace Racer.Player
{
    public class RaceHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _lapsCounter; //Laps: 1 / N
        [SerializeField]
        private TextMeshProUGUI _checkpointsCounter; //Checkpoints: 0 / N
        /// <summary>
        /// Shows time of current lap (updates every frame)
        /// </summary>
        [SerializeField, Tooltip("Shows time of current lap (updates every frame)")]
        private TextMeshProUGUI _currentLapTimeText; //Lap Time: 00.00.0
        [SerializeField]
        private TextMeshProUGUI _totalTimeText; //Race Time: 00.00.0

        [SerializeField]
        private Judge _playerJudge;

        private void Awake()
        {
            if (_playerJudge != null)
            {
                EnableScript();
            }
        }

        private void Start()
        {
            UpdateRaceInfo();
        }

        private void OnDestroy()
        {
            if (_playerJudge != null)
            {
                DisableScript();
            }
        }

        public void EnableScript()
        {
            _playerJudge.CollectedCheckpointEvent += UpdateRaceInfo;
            _playerJudge.FinishedLapEvent += UpdateRaceInfo;
        }

        public void DisableScript()
        {
            _playerJudge.CollectedCheckpointEvent -= UpdateRaceInfo;
            _playerJudge.FinishedLapEvent -= UpdateRaceInfo;
        }

        public void SetJudge(Judge judge)
        {
            _playerJudge = judge;
            EnableScript();
            UpdateRaceInfo();
        }

        private void UpdateRaceInfo()
        {
            if (_playerJudge != null)
            {
                if (_playerJudge.GetLapsDone >= _playerJudge.TotalLaps) _lapsCounter.text = $"Laps: {_playerJudge.TotalLaps} / {_playerJudge.TotalLaps}";
                else _lapsCounter.text = $"Laps: {_playerJudge.GetLapsDone + 1} / {_playerJudge.TotalLaps}";
                _checkpointsCounter.text = $"Checkpoints {_playerJudge.CheckpointsCounter} / {_playerJudge.TotalCheckpoints}";
            }
        }

        private void UpdateStopwatch()
        {
            var currentTime = _playerJudge.CurrentRaceTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            int minutes = (int)time.TotalMinutes;
            if (minutes > 99) minutes = 99;
            _totalTimeText.text = $"Race Time: {minutes}:{time.ToString(@"ss\:f")}";

            currentTime = _playerJudge.CurrentLapTime;
            time = TimeSpan.FromSeconds(currentTime);
            minutes = (int)time.TotalMinutes;
            if (minutes > 99) minutes = 99;
            _currentLapTimeText.text = $"Lap Time: { minutes}:{ time.ToString(@"ss\:f")}";
        }

        private void Update()
        {
            UpdateStopwatch();
        }
    }
}