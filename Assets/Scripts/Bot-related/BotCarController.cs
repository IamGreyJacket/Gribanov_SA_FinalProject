using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.AI
{
    public class BotCarController : BaseController
    {
        private const float c_convertMeterPerSecToKmPH = 3.6f;

        private BotCheckpoint[] _botCheckpoints;
        private Dictionary<BotCheckpoint, bool> _botCheckpointsProgress = new Dictionary<BotCheckpoint, bool>();
        private int _currentCheckpoint = 0;

        public float TargetSpeed { get; set; }

        private float _steering;

        [SerializeField]
        private CarComponent _car;

        private void Start()
        {
            
#if UNITY_EDITOR
            StartCoroutine(Debugger());
#endif
        }
        private void FixedUpdate()
        {
            CalculateSteering();

            CalculateBraking();
            CalculateAcceleration();
        }

        private void CalculateSteering()
        {
            var checkpointPos = _botCheckpoints[_currentCheckpoint].transform.position;
            checkpointPos.y = transform.position.y;

            var direction = checkpointPos - transform.position;

            var angle = Vector3.SignedAngle(direction, transform.forward, transform.up);
            if (angle > _car.MaxSteerAngle) angle = _car.MaxSteerAngle;
            else if (angle < -_car.MaxSteerAngle) angle = -_car.MaxSteerAngle;
            Rotate = -(angle / _car.MaxSteerAngle);
        }

        private void CalculateBraking()
        {
            if (TargetSpeed == -1) return;
            var speedDifference = _car.RawSpeed * c_convertMeterPerSecToKmPH - TargetSpeed;
            if (speedDifference > 5) Brakes = 1f;
            else Brakes = 0f;
        }

        private void CalculateAcceleration()
        {
            if(TargetSpeed == -1)
            {
                Acceleration = 1f;
                if (_isCutoff) Acceleration = 0f;
                return;
            }
            var accel = (_car.RawSpeed * c_convertMeterPerSecToKmPH) / TargetSpeed;
            if (Brakes > 0 || accel > 1 || _isCutoff) Acceleration = 0f;
            else if (accel < 1) Acceleration = 1f;
        }
#if UNITY_EDITOR
        #region EDITOR
        private IEnumerator Debugger()
        {
            while (true)
            {
                Debug.Log($"Acceleration: {Acceleration} | Steering: {Rotate} | Brakes: {Brakes} | AllowedToMove: {_car.IsAllowedToMove} | IsCutoff: {_isCutoff}" +
                    $"\nCheckpoint: {_botCheckpoints[_currentCheckpoint].name}");
                yield return new WaitForSeconds(1f);
            }
        }
        #endregion
#endif

        public void NextCheckpoint(BotCheckpoint hitCheckpoint)
        {
            /*
            if (_botCheckpointsProgress[hitCheckpoint] == false) 
            {
                TargetSpeed = hitCheckpoint.TargetSpeed;
                _botCheckpointsProgress[hitCheckpoint] = true;
                _currentCheckpoint++;
            }
            else
            {

            }
            */
            TargetSpeed = hitCheckpoint.TargetSpeed;
            _currentCheckpoint++;
            if (_currentCheckpoint == _botCheckpoints.Length) _currentCheckpoint = 0;
        }

        public void SetCheckpoints(BotCheckpoint[] botCheckpoints)
        {
            _botCheckpoints = botCheckpoints;
            foreach (var check in _botCheckpoints)
            {
                _botCheckpointsProgress[check] = false;
            }
            //
            TargetSpeed = _botCheckpoints[_currentCheckpoint].TargetSpeed;
        }
    }
}