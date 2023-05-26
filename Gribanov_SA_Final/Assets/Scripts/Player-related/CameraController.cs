using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Racer.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _playerCamera;
        [SerializeField]
        private Rigidbody _rigidbody;
        /*
        [SerializeField]
        private Transform[] _cameraPositions;
        */
        [SerializeField]
        private Transform _thirdPersonPosition;
        [SerializeField]
        private Transform _bonnetPosition;
        private Transform _currentPositionTransform;
        //private byte _currentPosition = 0;

        [SerializeField, Min(0f)]
        private float _smoothTime;

        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            //_currentPositionTransform = _cameraPositions[_currentPosition];
            _currentPositionTransform = _thirdPersonPosition;

            _playerCamera.transform.SetParent(null);

            StartCoroutine(ChangeCameraCheck());
        }

        private void FixedUpdate()
        {
            _velocity = _rigidbody.velocity;
            _playerCamera.transform.position = Vector3.SmoothDamp(_playerCamera.transform.position, _currentPositionTransform.position, ref _velocity, _smoothTime);
            _playerCamera.transform.rotation = _currentPositionTransform.rotation;
        }

        private IEnumerator ChangeCameraCheck()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    /*
                    _currentPosition++;
                    if (_currentPosition == _cameraPositions.Length) _currentPosition = 0;
                    _currentPositionTransform = _cameraPositions[_currentPosition];
                    */
                    if (_currentPositionTransform == _thirdPersonPosition)
                    {
                        _currentPositionTransform = _bonnetPosition; //todo
                        _playerCamera.transform.SetParent(_currentPositionTransform);
                    }
                    else if (_currentPositionTransform == _bonnetPosition)
                    {
                        _currentPositionTransform = _thirdPersonPosition; //todo
                        _playerCamera.transform.SetParent(null);
                    }
                    _playerCamera.transform.position = _currentPositionTransform.position;
                    _playerCamera.transform.rotation = _currentPositionTransform.rotation;
                    //if (!IsSmooth) _playerCamera.transform.SetParent(_currentPositionTransform);
                    
                    yield return new WaitForSeconds(.5f);
                }
                yield return null;
            }
        }
    }
}