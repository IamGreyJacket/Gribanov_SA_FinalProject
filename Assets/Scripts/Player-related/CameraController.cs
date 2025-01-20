using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Racer.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraSpace;
        [SerializeField]
        private Transform _playerCamera;
        /*
        [SerializeField]
        private Transform[] _cameraPositions;
        */
        [SerializeField]
        private Transform _thirdPersonPosition;
        [SerializeField]
        private Transform _bonnetPosition;
        private Transform _currentPositionTransform;

        private CarComponent _car;

        public Transform ThirdPerson => _thirdPersonPosition;
        public Transform Bonnet => _bonnetPosition;
        //private byte _currentPosition = 0;

        [SerializeField, Range(0f, 1f)]
        private float _positionSmoothTime = .6f;
        [SerializeField, Range(0f, 1f)]
        private float _rotationSmoothTime = .95f;

        private bool _isAcceptable = false;

        
        private void Awake()
        {
            CheckAcceptance();
        }

        public void CheckAcceptance()
        {
            _isAcceptable = false;
            if (_playerCamera == null) return;
            if (_cameraSpace == null) return;
            if (_car.Rigidbody == null) return;
            if (_thirdPersonPosition == null) return;
            if (_bonnetPosition == null) return;

            _isAcceptable = true;
            _currentPositionTransform = ThirdPerson;
        }


        public void SetCamera(Camera camera)
        {
            _playerCamera = camera.transform;
            _cameraSpace = camera.transform.parent;
            _cameraSpace.position = _car.transform.position;
            _cameraSpace.rotation = _car.transform.rotation;
            _currentPositionTransform = _thirdPersonPosition;
            _playerCamera.localPosition = _currentPositionTransform.localPosition;
            _playerCamera.localRotation = _currentPositionTransform.localRotation;
            _cameraSpace.SetParent(null);

            StartCoroutine(ChangeCameraCheck());
            CheckAcceptance();
        }

        public void SetCar(CarComponent car)
        {
            _car = car;
            CheckAcceptance();
        }

        private void FixedUpdate()
        {
            if (_isAcceptable)
            {
                if (_currentPositionTransform == _thirdPersonPosition)
                {
                    //_velocity = _rigidbody.velocity;

                    var targetPosition = Vector3.Lerp(_cameraSpace.position,
                        _car.transform.position, 1 - _positionSmoothTime);

                    var targetRotation = Quaternion.Slerp(_cameraSpace.rotation,
                        _car.transform.rotation, 1 - _rotationSmoothTime);

                    _cameraSpace.position = targetPosition;
                    _cameraSpace.rotation = targetRotation;

                    //todo: remake the whole cameraController.
                }
                else
                {
                    _cameraSpace.position = _car.transform.position;
                    _cameraSpace.rotation = _car.transform.rotation;
                }
            }
        }

        private IEnumerator ChangeCameraCheck()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.C) && _isAcceptable)
                {
                    /*
                    _currentPosition++;
                    if (_currentPosition == _cameraPositions.Length) _currentPosition = 0;
                    _currentPositionTransform = _cameraPositions[_currentPosition];
                    */
                    if (_currentPositionTransform == _thirdPersonPosition)
                    {
                        _currentPositionTransform = _bonnetPosition; //todo
                    }
                    else if (_currentPositionTransform == _bonnetPosition)
                    {
                        _currentPositionTransform = _thirdPersonPosition; //todo
                    }
                    _playerCamera.transform.localPosition = _currentPositionTransform.localPosition;
                    _playerCamera.transform.localRotation = _currentPositionTransform.localRotation;
                    //if (!IsSmooth) _playerCamera.transform.SetParent(_currentPositionTransform);
                    
                    //yield return new WaitForSeconds(.5f);
                }
                yield return null;
            }
        }
    }
}