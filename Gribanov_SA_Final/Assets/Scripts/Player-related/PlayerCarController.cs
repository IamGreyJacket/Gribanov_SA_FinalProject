using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEditor;

namespace Racer.Player
{
    public class PlayerCarController : BaseController
    {

        private PlayerControls _controls;
        public PlayerControls Controls => _controls;
        [SerializeField, Min(1)]
        private float _steeringSpeed = 3f;

        #region Controls enabling/disabling
        private void Awake()
        {
            _controls = new PlayerControls();
            _controls.Car.GearUp.performed += _ => OnGearboxUse(true);
            _controls.Car.GearDown.performed += _ => OnGearboxUse(false);
            _controls.Car.HandBrake.performed += _ => OnHandbrakeUse(true);
            _controls.Car.HandBrake.canceled += _ => OnHandbrakeUse(false);
            _controls.Car.Flip.performed += OnFlip;
            _controls.Enable();
        }

        private void Start()
        {
            //

        }

        private void OnEnable()
        {
            if (_controls != null)
            {
                _controls.Enable();
            }
        }

        private void OnDisable()
        {
            if (_controls != null)
            {
                _controls.Disable();
            }
        }

        private void OnDestroy()
        {
            _controls.Car.Flip.performed -= OnFlip;
            //
            _controls.Dispose();
        }
        #endregion

        private void FixedUpdate()
        {
            var direction = _controls.Car.Steering.ReadValue<float>();
            if (direction == 0f && Rotate != 0f)
            {
                Rotate = Rotate > 0f
                    ? Rotate - Time.fixedDeltaTime * (_steeringSpeed / 2)
                    : Rotate + Time.fixedDeltaTime * (_steeringSpeed / 2);
                if (Rotate < .05f && Rotate > -.05f) Rotate = 0f;
            }
            else
            {
                Rotate = Mathf.Clamp(Rotate + direction * Time.fixedDeltaTime * _steeringSpeed, -1f, 1f);
            }

            Brakes = _controls.Car.Brakes.ReadValue<float>();

            if (!_isCutoff)
            {
                Acceleration = _controls.Car.Acceleration.ReadValue<float>();
            }
            
        }

    }
}