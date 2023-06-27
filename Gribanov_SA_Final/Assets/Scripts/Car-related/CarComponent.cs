using Racer.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Racer
{
    [RequireComponent(typeof(WheelsComponent), typeof(Rigidbody))]
    public class CarComponent : MonoBehaviour
    {
        public event Action OnGearChangeEvent;

        [SerializeField]
        private BaseController _input;
        private WheelsComponent _wheels;
        private WheelCollider[] _driveWheels;
        private Rigidbody _rigidbody;
        public float Speed => _rigidbody.velocity.magnitude;

        [Space, Header("Transmission"), SerializeField]
        private DrivetrainType _drivetrain;
        [SerializeField]
        private bool _isAutomatic = true;
        [SerializeField, Tooltip("For how much time should power be cutoff, after shifting gears")]
        private float _powerDelay = 0f;
        [SerializeField, Min(0), Tooltip("Engine's RPM, at which gearbox shifts down")]
        private float _gearDownRPM = 2500;
       
        [SerializeField]
        private float[] _gears; //[0] - reverse, [1] - neutral, [2] - first gear, etc.
        private byte _currentGear = 2; //0 - reverse, 1 - neutral, 2 - first gear, etc.
        public byte CurrentGear => _currentGear;

        [Space, Header("Engine"), SerializeField, Min(0)]
        private float _engineTorque = 420f; //todo
        [SerializeField, Min(1200)]
        private float _maxRPM = 4500; //RPM limiter
        private float _engineRPM = 0;
        public float MaxRPM => _maxRPM;
        public float EngineRPM => _engineRPM;
        [SerializeField]
        private AnimationCurve _enginePowerCurve = new AnimationCurve();
        private float _currentPower;

        private float _wheelsRPM;

        [Space, Header("Handling and braking"), SerializeField, Range(10f, 40f)]
        private float _maxSteerAngle = 25f;
        public float MaxSteerAngle => _maxSteerAngle;
        [SerializeField, Range(0f, float.MaxValue), Tooltip("Сила ручного тормоза")]
        private float _handBrakeTorque = float.MaxValue;
        [SerializeField, Range(0f, float.MaxValue), Tooltip("Сила тормозов, при нажатии на педаль тормоза")]
        private float _brakesTorque = float.MaxValue;
        private bool _isBraking = false;
        [SerializeField]
        private Vector3 _centerOfMass;

        [Space, Header("Visuals"), SerializeField]
        private Image[] _tailLightsImages;
        [SerializeField]
        private Light[] _tailLights;
        private float _alphaBeforeChanging;
        private float _lightInstensityBeforeChanging;

        private bool _isAcceptable = false;

        public bool IsAllowedToMove { get; set; } = false;

        private void FixedUpdate()
        {
            if (_isAcceptable)
            {
                _wheels.UpdateVisual(_input.Rotate * _maxSteerAngle);
                OnBrake();

                //todo insert RPM calculator
                UpdateWheelsRPM();
                UpdateEngineRPM();
                if (IsAllowedToMove)
                {
                    if (_input.Acceleration < 0 && _currentGear != 0)
                    {
                        _currentGear = 0;
                        OnGearChangeEvent?.Invoke();
                    }
                    else if (_input.Acceleration > 0 && _currentGear < 2)
                    {
                        _currentGear = 2;
                        OnGearChangeEvent?.Invoke();
                    }
                    _currentPower = _input.Acceleration * (_enginePowerCurve.Evaluate(_engineRPM) * _gears[_currentGear]);
                    //if (_currentPower < 0) _currentPower /= 2; //todo since there will be a reverse gear
                    for (int i = 0; i < _driveWheels.Length; i++)
                    {
                        _driveWheels[i].motorTorque = _currentPower;
                    }
                }
            }
        }

        private void UpdateEngineRPM()
        {
            float velocity = _rigidbody.velocity.magnitude;
            _engineRPM = Mathf.SmoothDamp(_engineRPM, 800 + (Mathf.Abs(_wheelsRPM) * 3.6f * (_gears[_currentGear])), ref velocity, 0.05f);
            if (_engineRPM >= _maxRPM) _engineRPM = _maxRPM;
        }

        private void UpdateWheelsRPM()
        {
            float sum = 0;
            for(int i = 0; i < _driveWheels.Length; i++)
            {
                sum += _driveWheels[i].rpm;
            }
            _wheelsRPM = (_driveWheels.Length != 0) ? sum / _driveWheels.Length : 0;
        }

        private void OnGearChange(bool value)
        {
            switch (value)
            {
                case true:
                    if (_currentGear + 1 >= _gears.Length) return;
                    _currentGear++;
                    break;
                case false:
                    if (_currentGear - 1 < 0) return;
                    _currentGear--;
                    break;
            }
            OnGearChangeEvent?.Invoke();
        }

        private void OnBrake()
        {
            if (_input.Brakes == 0f && _isBraking == true)
            {
                _isBraking = false;
                //погасить фары
                for (int i = 0; i < _tailLightsImages.Length; i++)
                {
                    var color = _tailLightsImages[i].color;
                    color.a = _alphaBeforeChanging;
                    _tailLightsImages[i].color = color;
                    _tailLights[i].intensity = _lightInstensityBeforeChanging;
                }
            }
            else if (_input.Brakes >= 0.05f && _isBraking == false)
            {
                _isBraking = true;
                //подсветить фары
                for (int i = 0; i < _tailLightsImages.Length; i++)
                {
                    var color = _tailLightsImages[i].color;
                    _alphaBeforeChanging = color.a;
                    color.a = 1f;
                    _tailLightsImages[i].color = color;
                    _lightInstensityBeforeChanging = _tailLights[i].intensity;
                    _tailLights[i].intensity *= 2;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                _wheels.GetFrontWheels[i].brakeTorque = _input.Brakes * _brakesTorque;
            }
        }

        private void OnHandBrake(bool value)
        {
            if (value)
            {
                for (int i = 0; i < 2; i++)
                {
                    _wheels.GetRearWheels[i].brakeTorque = _handBrakeTorque;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    _wheels.GetRearWheels[i].brakeTorque = 0;
                }
            }
        }

        private void OnFlip()
        {
            var pos = transform.position;
            pos.y += 1;
            transform.position = pos;
            transform.Rotate(new Vector3(0, 0, 180));

        }

        private void PowerDelay()
        {
            StartCoroutine(_input.PowerDelay(_powerDelay));
        }

        private IEnumerator AutoGearbox()
        {
            while (true)
            {
                if (_isAutomatic)
                {
                    if ((_currentGear != _gears.Length - 1 & _currentGear != 0) && _engineRPM > _maxRPM - 200)
                    {
                        OnGearChange(true);
                        yield return new WaitForSeconds(.5f);
                    }
                    else if (_engineRPM < _gearDownRPM && _currentGear > 2) OnGearChange(false);
                    yield return new WaitForSeconds(.25f);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var point = _centerOfMass;
            point.z *= transform.localScale.z;
            point.x *= transform.localScale.x;
            //_centerOfMass.x *= transform.localScale.x;
            //_centerOfMass.z *= transform.localScale.z;

            Gizmos.DrawSphere(transform.TransformPoint(point), 0.4f);
        }

        private void Awake()
        {
            _wheels = GetComponent<WheelsComponent>();
            _rigidbody = GetComponent<Rigidbody>();

            OnGearChangeEvent += PowerDelay;
            if (_input != null)
            {
                _input.OnGearboxEvent += OnGearChange;
                _input.OnHandbrakeEvent += OnHandBrake;
                _input.OnFlipEvent += OnFlip;
                _isAcceptable = true;
            }
        }
        private void Start()
        {
            switch (_drivetrain)
            {
                case DrivetrainType.FrontWheelDrive:
                    _driveWheels = _wheels.GetFrontWheels;
                    break;
                case DrivetrainType.RearWheelDrive:
                    _driveWheels = _wheels.GetRearWheels;
                    break;
                case DrivetrainType.AllWheelDrive:
                    _driveWheels = _wheels.GetAllWheels;
                    break;
            }

            _rigidbody.centerOfMass = _centerOfMass;

            _currentGear = 2;
            StartCoroutine(AutoGearbox());
        }

        private void OnDestroy()
        {
            OnGearChangeEvent -= PowerDelay;
            if (_input != null)
            {
                _input.OnFlipEvent -= OnFlip;
                _input.OnHandbrakeEvent -= OnHandBrake;
                _input.OnGearboxEvent -= OnGearChange;
            }
        }
    }

    public enum DrivetrainType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }
}