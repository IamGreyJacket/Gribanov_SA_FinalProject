using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers.Assistants
{
    public class Judge : MonoBehaviour
    {
        public event Action CollectedCheckpointEvent;
        public event Action FinishedLapEvent;

        public void OnFinishLap()
        {
            RecordLapStopwatch();
            _lapCounter++;
            FinishedLapEvent?.Invoke();
            CheckpointsCounter = 0;
        }

        [SerializeField]
        private CarComponent _car;

        //For time-attack
        private bool _isStopwatch;
        private float _currentRaceTime;
        public float CurrentRaceTime => _currentRaceTime;
        private float _currentLapTime;
        public float CurrentLapTime => _currentLapTime;
        private List<float> _lapTimes = new List<float>();
        public List<float> LapTimes => _lapTimes;


        //For circuit races
        private Dictionary<Checkpoint, bool> _checkpointStatus = new Dictionary<Checkpoint, bool>();
        public int TotalCheckpoints => _checkpointStatus.Count;
        public byte CheckpointsCounter { get; private set; }
        private byte _laps;
        /// <summary>
        /// Laps, needed to be done to finish
        /// </summary>
        public byte TotalLaps => _laps;
        private byte _lapCounter = 0;
        public byte GetLapsDone => _lapCounter;

        #region Time-Attack Race Params
        private float _timeToBeat;
        public float TimeToBeat => _timeToBeat;
        #endregion

        private void Awake()
        {
            _car.IsAllowedToMove = false;
        }

        public void StartRace()
        {
            _car.IsAllowedToMove = true;
            StartStopwatch();
        }

        public void EndRace()
        {
            _car.IsAllowedToMove = false;
        }

        private void Update()
        {
            if (_isStopwatch)
            {
                _currentRaceTime += Time.deltaTime;
                _currentLapTime += Time.deltaTime;
            }
        }

        public void SetRaceParams(Checkpoint[] checkpoints, byte laps, float timeToBeat) 
        {
            foreach (var check in checkpoints)
            {
                _checkpointStatus[check] = false;
            }
            _laps = laps;
            _timeToBeat = timeToBeat;
        }

        public void UpdateCheckpoint(Checkpoint checkpoint, bool status)
        {
            if (_checkpointStatus[checkpoint] == true) return;
            _checkpointStatus[checkpoint] = status;
            if (status == true)
            {
                CheckpointsCounter++;
                CollectedCheckpointEvent?.Invoke();
                //Debug.Log($"{checkpoint.name} collected by {_car.name}");
            }
        }

        /// <summary>
        /// Returns TRUE if ALL checkpoints are collected, otherwise returns FALSE
        /// </summary>
        /// <returns></returns>
        public bool GetCheckpointsStatus()
        {
            bool isCollected = true;
            foreach (var check in _checkpointStatus.Values)
            {
                if (check == false) isCollected = false;
            }
            if (isCollected == true) CheckpointsCounter = 0;
            return isCollected;
        }

        public void StartStopwatch()
        {
            _currentRaceTime = 0;
            _currentLapTime = 0;
            _isStopwatch = true;
            //Debug.Log("Stopwatch is running");
        }

        public void RecordLapStopwatch()
        {
#if UNITY_EDITOR
            var currentTime = CurrentLapTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            int minutes = (int)time.TotalMinutes;
            if (minutes > 99) minutes = 99;
            Debug.Log($"Lap {_lapCounter + 1} Time: { minutes}:{ time.ToString(@"ss\:f")}");
#endif

            _lapTimes.Add(_currentLapTime);
            if (GetLapsDone == TotalLaps) return;
            _currentLapTime = 0;
        }

        public void StopStopwatch()
        {
            _isStopwatch = false;
            //Debug.Log($"Stopwatch is stopped at {_currentRaceTime}");
        }
    }
}