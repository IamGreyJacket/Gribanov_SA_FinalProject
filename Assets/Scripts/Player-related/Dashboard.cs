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

        private void Update()
        {
            UpdateSpeed();
            UpdateRPM();
            UpdateRPMNeedle();
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
            //_rpmText.color = Color.Lerp(_minColor, _maxColor, rpm / _maxRPM);
            _rpmText.text = $"{(int)_car.EngineRPM} RPM";
        }

        private void UpdateRPMNeedle()
        {
            //if (!_isAcceptable) return;
            var angles = _rpmNeedle.transform.eulerAngles;
            angles.z = Mathf.Lerp(_minRPMRotation, _maxRPMRotation, _car.EngineRPM / _car.MaxRPM);
            _rpmNeedle.transform.eulerAngles = angles;
        }

        private void UpdateSpeed()
        {
            //if (!_isAcceptable) return;
            var speed = Mathf.Round(_car.Speed * c_convertMeterPerSecToKmPH);
            _speedText.color = Color.Lerp(_minColor, _maxColor, speed / _maxSpeed);
            _speedText.text = $"{(int)speed} km/h";
        }

        private void UpdateGears()
        {
            if (_car.CurrentGear == 0) _gearsText.text = "R";
            else if (_car.CurrentGear == 1) _gearsText.text = "N";
            else _gearsText.text = (_car.CurrentGear - 1).ToString();
        }
    }
}