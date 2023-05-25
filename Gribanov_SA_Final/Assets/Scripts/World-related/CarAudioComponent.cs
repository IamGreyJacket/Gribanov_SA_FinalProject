using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers.Assistants
{
    public class CarAudioComponent : MonoBehaviour
    {
        [SerializeField]
        AudioSource _audioSource;

        [Space, SerializeField]
        private AudioClip _engineIdle;
        [SerializeField]
        private AudioClip _engineRunning;

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
            if (_audioSource == null) Debug.Log("No Audio Source");
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
            _audioSource.pitch = _audioPitch;
        }

        private void CalculatePitch()
        {
            var engineRPMRatio = _car.EngineRPM / _car.MaxRPM; //значение не нулевое
            _audioPitch = _maxAudioPitch * engineRPMRatio;
            if (_audioPitch < _minAudioPitch || _car.EngineRPM < _idleRPM)
            {
                _audioSource.clip = _engineIdle;
                _audioPitch = _idlePitch;
            }
            else
            {
                _audioSource.clip = _engineRunning; //
            }
            if (!_audioSource.isPlaying) _audioSource.Play();
        }

    }
}