using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer
{
    public class VisualTuningComponent : MonoBehaviour
    {
        [SerializeField]
        private WheelsComponent _wheelsComponent;
        [SerializeField]
        private MeshFilter _spoilerMeshFilter;
        [SerializeField]
        private Transform _spoilerTransform;

        public MeshFilter[] LeftWheelsMeshFilter => _wheelsComponent.GetLeftMeshFilters;
        public MeshFilter[] RightWheelsMeshFilter => _wheelsComponent.GetRightMeshFilters;

        private int _wheelsCurrentID = 0;
        private int _spoilerCurrentID = 0;
        public int WheelsCurrentID => _wheelsCurrentID;
        public int SpoilerCurrentID => _spoilerCurrentID;

        [Space, Header("Wheels and spoilers choice"), SerializeField]
        private Mesh[] _leftWheelMeshes;
        [SerializeField]
        private Mesh[] _rightWheelMeshes;
        [SerializeField]
        private Material[] _WheelMeshMaterials;
        [SerializeField]
        private Mesh[] _spoilerMeshes;

        public Mesh[] LeftWheelMeshes => _leftWheelMeshes;
        public Mesh[] SpoilerMeshes => _spoilerMeshes;


        public void SetWheelMeshFromID(int wheelID, Vector3 scale)
        {
            if (wheelID > _leftWheelMeshes.Length ||
                (_leftWheelMeshes[_wheelsCurrentID] == _leftWheelMeshes[wheelID] &&
                _WheelMeshMaterials[wheelID] == LeftWheelsMeshFilter[0].GetComponent<MeshRenderer>().material)) return;
            var leftWheelsMeshFilters = LeftWheelsMeshFilter;
            var rightWheelsMeshFilters = RightWheelsMeshFilter;
            for (int i = 0; i < leftWheelsMeshFilters.Length; i++)
            {
                if (wheelID == 0)
                {
                    leftWheelsMeshFilters[i].mesh = _wheelsComponent.WheelsOriginalLeftMesh;//_leftWheelMeshes[wheelID];
                    leftWheelsMeshFilters[i].GetComponent<MeshRenderer>().material = _wheelsComponent.WheelsOriginalMaterial;
                    rightWheelsMeshFilters[i].mesh = _wheelsComponent.WheelsOriginalRightMesh;//_rightWheelMeshes[wheelID];
                    rightWheelsMeshFilters[i].GetComponent<MeshRenderer>().material = _wheelsComponent.WheelsOriginalMaterial;
                }
                else
                {
                    leftWheelsMeshFilters[i].mesh = _leftWheelMeshes[wheelID];
                    leftWheelsMeshFilters[i].GetComponent<MeshRenderer>().material = _WheelMeshMaterials[wheelID];
                    rightWheelsMeshFilters[i].mesh = _rightWheelMeshes[wheelID];
                    rightWheelsMeshFilters[i].GetComponent<MeshRenderer>().material = _WheelMeshMaterials[wheelID];
                }
            }
            foreach (var tranformWheel in _wheelsComponent.GetFrontTransform)
            {
                if (wheelID == 0) tranformWheel.localScale = new Vector3(100, 100, 100);
                else tranformWheel.localScale = scale;
            }
            foreach (var tranformWheel in _wheelsComponent.GetRearTransform)
            {
                if (wheelID == 0) tranformWheel.localScale = new Vector3(100, 100, 100);
                else tranformWheel.localScale = scale;
            }
            _wheelsCurrentID = wheelID;
        }

        public void SetSpoilerMeshFromID(int spoilerID)
        {
            if (spoilerID > _spoilerMeshes.Length || _spoilerMeshFilter.mesh == _spoilerMeshes[spoilerID]) return;
            _spoilerMeshFilter.mesh = _spoilerMeshes[spoilerID];
            _spoilerCurrentID = spoilerID;
        }
    }
}