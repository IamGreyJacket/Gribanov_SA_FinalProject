using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Racer.Managers
{
    public class GameManager : MonoBehaviour
    {

        public event Action OnEscapeEvent;

        private byte _laps = 1;
        private byte _bots;

        public void SetLaps(string laps)
        {
            _laps = Convert.ToByte(laps);
            Debug.Log($"Laps: {_laps}");
        }

        public void SetBots(string bots)
        {
            _bots = Convert.ToByte(bots);
            Debug.Log($"Bots: {_bots}");
        }

        public Player.PlayerSave PlayerSave;

        public static GameManager Self { get; private set; }

        [SerializeField]
        private TrackManager _trackManager;
        [SerializeField]
        private Racer.AI.BotManager _botManager;
        [SerializeField]
        private ResultsManager _resultsManager;
        [SerializeField]
        private Menu.MenuManager _menuManager;
        [SerializeField]
        private AudioManager _audioManager;

        public ResultsManager ResultsManager => _resultsManager;

        private PlayerControls _controls;

        private void Awake()
        {
            if (Self == null)
            {
                Self = this;
                DontDestroyOnLoad(this);

                _controls = new PlayerControls();
                _controls.Base.Escape.performed += OnEscape;
                OnEscapeEvent += LoadMainMenu;
                _controls.Enable();
                FindManagers();
                SetManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            OnEscapeEvent -= LoadMainMenu;
            if (_controls != null)
            {
                _controls.Base.Escape.performed -= OnEscape;
                _controls.Disable();
                _controls.Dispose();
            }
        }

        private void OnEscape(InputAction.CallbackContext obj)
        {
            Debug.Log("Escape is pressed");
            OnEscapeEvent?.Invoke();
        }

        private void OnLevelWasLoaded(int level)
        {
            FindManagers();
            SetManagers();
        }

        public void SetManagers()
        {
            if (_trackManager != null)
            {
                _trackManager.Laps = _laps;
                if (_trackManager.RaceType == RaceType.TimeAttack) _trackManager.Laps = 3;
                Debug.Log($"_trackManager.Laps: {_trackManager.Laps}. _laps: {_laps}");
                _trackManager.SetJudges();
            }
            if (_botManager != null)
            {

            }
            if (_resultsManager != null)
            {

            }
            if (_menuManager != null)
            {
                _menuManager.SetPlayerSave(PlayerSave);
                //открывать или закрывать доступ в соответствии с сейвом
            }
            if(_audioManager != null)
            {
                _audioManager.SetVolume(PlayerSave.SoundVolume);
            }
        }

        public void FindManagers()
        {
            _trackManager = null;
            _botManager = null;
            _resultsManager = null;
            _menuManager = null;
            _audioManager = null;

            _trackManager = FindObjectOfType<TrackManager>();
            _botManager = FindObjectOfType<AI.BotManager>();
            _resultsManager = FindObjectOfType<ResultsManager>();
            _menuManager = FindObjectOfType<Menu.MenuManager>();
            _audioManager = FindObjectOfType<AudioManager>();
        }

        public void StartRace()
        {
            _trackManager.StartRace();
        }

        public async void LoadMainMenu()
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenuScene")) return;
            var scene = SceneManager.LoadSceneAsync("MainMenuScene");
            scene.allowSceneActivation = false;
            while (scene.progress < .9f)
            {
                await Task.Delay(100);
            }
            scene.allowSceneActivation = true;
        }

        public void SetVolume(float volume)
        {
            PlayerSave.SoundVolume = volume;
            if (_audioManager != null)
            {
                _audioManager.SetVolume(PlayerSave.SoundVolume);
            }
        }
    }
}