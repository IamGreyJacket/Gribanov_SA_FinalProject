using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private float _soundVolume = 1f;

        private AudioSource[] _audioSources;

        public static AudioManager Self { get; set; }

        private void Awake()
        {
            if (Self == null)
            {
                Self = this;
                DontDestroyOnLoad(this);
                GetAudioSources();
                SetAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            GetAudioSources();
            SetAudioSources();
        }

        public void SetVolume(float volume) => _soundVolume = volume;
        

        public void GetAudioSources()
        {
            _audioSources = FindObjectsOfType<AudioSource>();
        }

        public void SetAudioSources()
        {
            foreach(var source in _audioSources)
            {
                source.volume = _soundVolume;
                Debug.Log($"{source.gameObject.name} volume is {source.volume}. AudioManager's volume is: {_soundVolume}");
            }
        }
    }
}