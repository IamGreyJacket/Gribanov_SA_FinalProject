using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers.Assistants
{
    public class CarAudioComponent : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _engineAudioSource;
        [SerializeField]
        private AudioSource _bodyAudioSource;
        [SerializeField]
        private AudioSource _wheelsAudioSource;

        [Space, SerializeField]
        private AudioClip _engineIdleAudio;
        [SerializeField]
        private AudioClip _engineRunningAudio;
        [SerializeField]
        private AudioClip _crashAudio;

        [Space, SerializeField, Min(0)]
        private float _minAudioPitch = 0.5f;
        [SerializeField, Min(0)]
        private float _maxAudioPitch = 1.6f;
        [SerializeField, Min(0)]
        private float _idleRPM = 1000f;
        [SerializeField, Min(0)]
        private float _idlePitch = 1f;
        private float _audioPitch = 1;

        [SerializeField]
        private CarComponent _car;

        private bool _isAcceptable = false;

        private void Awake()
        {
            if (_engineAudioSource == null) Debug.Log("No Audio Source");
            else
            {
                _isAcceptable = true;
                Debug.Log("Yes Audio Source");
            }
        }

        private void Update()
        {
            if (!_isAcceptable) return;
            CalculatePitch();
            _engineAudioSource.pitch = _audioPitch;
        }

        private void OnCollisionEnter(Collision collision)
        {
            _bodyAudioSource.PlayOneShot(_crashAudio);
        }

        public void SetVolume(float volume)
        {
            _engineAudioSource.volume = volume;
            _bodyAudioSource.volume = volume;
            _wheelsAudioSource.volume = volume;
        }

        private void CalculatePitch()
        {
            var engineRPMRatio = _car.EngineRPM / _car.MaxRPM; //значение не нулевое
            _audioPitch = _maxAudioPitch * engineRPMRatio;
            if (_audioPitch < _minAudioPitch || _car.EngineRPM < _idleRPM)
            {
                _engineAudioSource.clip = _engineIdleAudio;
                _audioPitch = _idlePitch;
            }
            else
            {
                _engineAudioSource.clip = _engineRunningAudio; //
            }
            if (!_engineAudioSource.isPlaying) _engineAudioSource.Play();
        }

    }
}