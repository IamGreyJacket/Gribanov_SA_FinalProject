using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _playerCamera;

        [SerializeField]
        private Transform[] _cameraPositions;
        private byte _currentCamera;

        private void LateUpdate()
        {
            
        }
    }
}