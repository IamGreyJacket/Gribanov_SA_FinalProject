using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer
{
    public class WheelsComponent : MonoBehaviour
    {
        private WheelCollider[] _allWheels;

        [SerializeField]
        private WheelCollider[] _frontWheelColliders;
        [SerializeField]
        private WheelCollider[] _rearWheelColliders;

        [Space, SerializeField]
        private Transform[] _frontTransform;
        [SerializeField]
        private Transform[] _rearTransform;

        public WheelCollider[] GetAllWheels => _allWheels;
        public WheelCollider[] GetFrontWheels => _frontWheelColliders;
        public WheelCollider[] GetRearWheels => _rearWheelColliders;
        public Transform[] GetFrontTransform => _frontTransform;
        public Transform[] GetRearTransform => _rearTransform;

        private void Awake()
        {
            _allWheels = new WheelCollider[] { _frontWheelColliders[0], _frontWheelColliders[1], _rearWheelColliders[0], _rearWheelColliders[1] };
        }

        public void UpdateVisual(float angle)
        {
            for (int i = 0; i < _frontTransform.Length; i++)
            {
                _frontWheelColliders[i].steerAngle = angle;

                _frontWheelColliders[i].GetWorldPose(out var pos, out var rot);

                _frontTransform[i].position = pos;
                _frontTransform[i].rotation = rot;
                
                _rearWheelColliders[i].GetWorldPose(out pos, out rot);

                _rearTransform[i].position = pos;
                _rearTransform[i].rotation = rot;
            }
        }
    }
}