using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private float _soundVolume = 1f;

        private List<AudioSource> _audioSources;
        private Dictionary<AudioSource, float> _originalVolume = new Dictionary<AudioSource, float>();

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

        private void OnEnable()
        {
            GameManager.Self.LevelLoaded += OnLevelLoaded;
        }

        private void OnDestroy()
        {
            GameManager.Self.LevelLoaded -= OnLevelLoaded;
        }

        public void SaveAudioSource(AudioSource source)
        {
            _originalVolume.Add(source, source.volume);
        }

        private void OnLevelLoaded()
        {
            GetAudioSources();
            SetAudioSources();
        }

        public void SetVolume(float volume)
        {
            _soundVolume = volume;
            GetAudioSources();
            SetAudioSources();
        }
        

        public void GetAudioSources()
        {
            _audioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
            _originalVolume.Clear();
        }

        public void SetAudioSources()
        {
            foreach(var source in _audioSources)
            {
                if (!_originalVolume.TryGetValue(source, out var original))
                {
                    _originalVolume.Add(source, source.volume);
                    original = _originalVolume[source];
                }

                source.volume = original * _soundVolume;
                Debug.Log($"{source.gameObject.name} volume is {source.volume}. AudioManager's volume is: {_soundVolume}");
            }
        }
    }
}