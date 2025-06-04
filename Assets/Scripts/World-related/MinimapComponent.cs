using Racer.AI;
using Racer.Managers.Assistants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Racer
{
    public class MinimapComponent : MonoBehaviour
    {
        [SerializeField]
        private Transform _worldPoint1;
        [SerializeField]
        private Transform _worldPoint2;
        [SerializeField]
        private RectTransform _minimapPoint1;
        [SerializeField]
        private RectTransform _minimapPoint2;
        [SerializeField]
        private Image _playerArrowOnMinimap; //can be used as prefab

        [Space, SerializeField]
        private Transform _minimap; //inside Canvas

        //
        private Dictionary<Transform, RectTransform> _participants = new Dictionary<Transform, RectTransform>(); //

        private float _minimapRatio;

        //private bool _isAcceptable = false;

        private void Awake()
        {
            CalculateMapRatio();
        }

        private void LateUpdate()
        {
            //if (_isAcceptable)
            
            foreach(var part in _participants.Keys)
            {
                var miniArrow = _participants[part];
                miniArrow.anchoredPosition = _minimapPoint1.anchoredPosition + new Vector2((part.position.x - _worldPoint1.position.x) * _minimapRatio,
                    (part.position.z - _worldPoint1.position.z) * _minimapRatio);
                var angles = miniArrow.eulerAngles;
                angles.z = -part.eulerAngles.y;
                miniArrow.eulerAngles = angles;
            }
            /*
            _playerArrowOnMinimap.anchoredPosition = _minimapPoint1.anchoredPosition + new Vector2((_playerTransform.position.x - _worldPoint1.position.x) * _minimapRatio,
                (_playerTransform.position.z - _worldPoint1.position.z) * _minimapRatio);
            var angles = _playerArrowOnMinimap.eulerAngles;
            angles.z = -_playerTransform.eulerAngles.y;
            _playerArrowOnMinimap.eulerAngles = angles;
            */
            
        }

        /*
        public void CheckAcceptance()
        {
            if (_playerTransform != null)
            {
                _isAcceptable = true;
            }
            else
            {
                _isAcceptable = false;
            }
        }
        */

        public void FindParticipants()
        {
            var participants = FindObjectsOfType<Judge>();
            foreach(var part in participants)
            {
                var miniArrow = Instantiate(_playerArrowOnMinimap, _minimap);
                if(part.GetComponent<BaseController>().GetType() == typeof(BotCarController))
                {
                    miniArrow.color = Color.red;
                }
                _participants.Add(part.transform, miniArrow.rectTransform);
            }
        }

        /*
        public void SetCar(CarComponent car)
        {
            _playerTransform = car.transform;
            CheckAcceptance();
        }
        */

        private void CalculateMapRatio()
        {
            //distance world ignoring Y axis
            Vector3 distanceWorldVector = _worldPoint1.position - _worldPoint2.position;
            distanceWorldVector.y = 0f;
            float distanceWorld = distanceWorldVector.magnitude;

            //distance minimap
            float distanceMinimap = Mathf.Sqrt(
                Mathf.Pow((_minimapPoint1.anchoredPosition.x - _minimapPoint2.anchoredPosition.x), 2) +
                Mathf.Pow((_minimapPoint1.anchoredPosition.y - _minimapPoint2.anchoredPosition.y), 2));

            _minimapRatio = distanceMinimap / distanceWorld;
        }
    }
}