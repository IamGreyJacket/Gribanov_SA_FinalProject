using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Racer.Player
{
    public class Dashboard : MonoBehaviour
    {
        private const float c_convertMeterPerSecToKmPH = 3.6f;

        [SerializeField]
        private float _maxSpeed = 300f;
        [SerializeField]
        private TextMeshProUGUI _speedText;

        [Space, SerializeField]
        private TextMeshProUGUI _gearsText;

        private float _maxRPM = 4500;
        [Space, SerializeField]
        private TextMeshProUGUI _rpmText;
        [SerializeField]
        private GameObject _rpmNeedle;
        [SerializeField]
        private float _minRPMRotation;
        [SerializeField]
        private float _maxRPMRotation;
        [SerializeField]
        private Color _minColor = Color.blue;
        [SerializeField]
        private Color _maxColor = Color.red;

        [Space, SerializeField]
        private CarComponent _car;

        private bool _isAcceptable = false;
        private void Start()
        {
            if (_speedText != null && _car != null)
            {
                _isAcceptable = true;
                _maxRPM = _car.MaxRPM;
                _car.OnGearChangeEvent += UpdateGears;
                UpdateGears();
            }
        }

        private void Awake()
        {
            CheckAcceptance();
        }

        private void FixedUpdate()
        {
            if (_isAcceptable)
            {
                UpdateSpeed();
                UpdateRPM();
                UpdateRPMNeedle();
            }
        }

        public void CheckAcceptance()
        {
            if (_speedText != null && _car != null)
            {
                if (_isAcceptable == false)
                {
                    _isAcceptable = true;
                    _maxRPM = _car.MaxRPM;
                    _car.OnGearChangeEvent += UpdateGears;
                    UpdateGears();
                }
            }
            else
            {
                _isAcceptable = false;
            }
        }

        public void SetCar(CarComponent car)
        {
            _car = car;
            CheckAcceptance();
        }

        private void UpdateRPM()
        {
            var rpm = (int)_car.EngineRPM;
            //_rpmText.color = Color.Lerp(_minColor, _maxColor, rpm / _maxRPM);
            _rpmText.text = $"{rpm} RPM";
        }

        private void UpdateRPMNeedle()
        {
            if (!_isAcceptable) return;
            var desiredPos = _minRPMRotation - _maxRPMRotation;
            float temp = _car.EngineRPM / _maxRPM;
            _rpmNeedle.transform.eulerAngles = new Vector3(0, 0, _minRPMRotation - temp * desiredPos);
        }

        private void UpdateSpeed()
        {
            if (!_isAcceptable) return;
            var speed = (float)System.Math.Round(_car.Speed * c_convertMeterPerSecToKmPH, 0);
            _speedText.color = Color.Lerp(_minColor, _maxColor, speed / _maxSpeed);
            _speedText.text = $"{speed} km/h";
        }

        private void UpdateGears()
        {
            if (_car.CurrentGear == 0) _gearsText.text = "R";
            else if (_car.CurrentGear == 1) _gearsText.text = "N";
            else _gearsText.text = (_car.CurrentGear - 1).ToString();
        }
    }
}