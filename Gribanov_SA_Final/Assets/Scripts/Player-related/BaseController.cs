using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Racer
{
    public class BaseController : MonoBehaviour
    {
        public event Action<bool> OnHandbrakeEvent;
        public event Action OnFlipEvent;
        public event Action<bool> OnGearboxEvent;

        protected bool _isCutoff = false;

        public float Acceleration { get; protected set; }
        public float Rotate { get; protected set; }
        public float Brakes { get; protected set; }

        protected void OnGearboxUse(bool value) => OnGearboxEvent?.Invoke(value);
        protected void OnHandbrakeUse(bool value) => OnHandbrakeEvent?.Invoke(value);
        protected void OnFlip(InputAction.CallbackContext obj) => OnFlipEvent?.Invoke();

        /// <summary>
        /// Reduces Acceleration to 0 for a fraction of a second
        /// </summary>
        /// <returns></returns>
        public IEnumerator PowerDelay(float powerDelay)
        {
            var time = powerDelay;
            _isCutoff = true;
            while (time > 0)
            {
                Acceleration = 0f;
                time -= Time.deltaTime;
                yield return null;
            }
            _isCutoff = false;
            yield return null;
        }
    }
}