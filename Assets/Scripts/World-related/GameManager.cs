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
        public event Action LevelLoaded;

        public Player.PlayerSaveSO PlayerSave;

        public static GameManager Self { get; private set; }
        private bool _isSelf = false;

        [SerializeField]
        private TrackManager _trackManager;
        [SerializeField]
        private Racer.AI.BotManager _botManager;
        [SerializeField]
        private ResultsManager _resultsManager;
        [SerializeField]
        private SaveManager _saveManager;
        [SerializeField]
        private Menu.MenuManager _menuManager;
        [SerializeField]
        private AudioManager _audioManager;

        [Space, SerializeField]
        private CarComponent _ladaPlayerCarPrefab;
        [SerializeField]
        private CarComponent _uazPlayerCarPrefab;
        [SerializeField]
        private CarComponent _volgaPlayerCarPrefab;
        [SerializeField]
        private CarComponent _ladaBotCarPrefab;
        [SerializeField]
        private CarComponent _uazBotCarPrefab;
        [SerializeField]
        private CarComponent _volgaBotCarPrefab;

        public CarComponent PlayerCar { get; set; }

        public TrackManager TrackManager => _trackManager;
        public AI.BotManager BotManager => _botManager;
        public ResultsManager ResultsManager => _resultsManager;
        public SaveManager SaveManager => _saveManager;
        public Menu.MenuManager MenuManager => _menuManager;
        public AudioManager AudioManager => _audioManager;

        private PlayerControls _controls;


        private void Awake()
        {
            if (Self == null)
            {
                Self = this;
                _isSelf = true;
                DontDestroyOnLoad(this);

                _controls = new PlayerControls();
                _controls.Base.Escape.performed += OnEscape;
                OnEscapeEvent += LoadMainMenu;
                _controls.Enable();
                FindManagers();
                if (_saveManager != null)
                {
                    var playerSave = _saveManager.GetSave();
                    if (playerSave != null)
                    {
                        PlayerSave.Info = JsonUtility.FromJson<Player.PlayerSaveInfo>(playerSave);
                        SaveGame();
                    }
                }
                SetPlayerCar(PlayerSave.Info.CarID);
                SetManagers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            LevelLoaded += OnLevelLoaded;
        }

        private void OnDisable()
        {
            LevelLoaded -= OnLevelLoaded;
        }

        private void OnDestroy()
        {
            if (_isSelf)
            {
                SaveGame();
                OnEscapeEvent -= LoadMainMenu;
                if (_controls != null)
                {
                    _controls.Base.Escape.performed -= OnEscape;
                    _controls.Disable();
                    _controls.Dispose();
                }
            }

        }

        private void OnEscape(InputAction.CallbackContext obj)
        {
            Debug.Log("Escape is pressed");
            OnEscapeEvent?.Invoke();
        }

        private void OnLevelLoaded()
        {
            if (_isSelf)
            {
                FindManagers();
                SetManagers();
            }
        }

        public async void LoadLevel(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);
            while (!scene.isDone)
            {
                await Task.Yield();
            }
            LevelLoaded?.Invoke();
        }

        public void SetPlayerCar(int carID)
        {
            switch (carID)
            {
                case 0:
                    PlayerCar = null;
                    PlayerSave.Info.CarID = carID;
                    break;
                case 1:
                    PlayerCar = _ladaPlayerCarPrefab;
                    PlayerSave.Info.CarID = carID;
                    break;
                case 2:
                    PlayerCar = _uazPlayerCarPrefab;
                    PlayerSave.Info.CarID = carID;
                    break;
                case 3:
                    PlayerCar = _volgaPlayerCarPrefab;
                    PlayerSave.Info.CarID = carID;
                    break;
            }
            Debug.Log($"{carID}, {PlayerCar}");
            SaveGame();
        }

        public void SetManagers()
        {
            if (_trackManager != null)
            {
                if (_trackManager.RaceType == RaceType.TimeAttack) _trackManager.Laps = 3;
                else if (_trackManager.RaceType == RaceType.Circuit) _trackManager.Laps = 3;
                Debug.Log($"_trackManager.Laps: {_trackManager.Laps}");
                _trackManager.PlayerCar = PlayerCar;
                if (PlayerCar == null) _trackManager.PlayerCar = _ladaPlayerCarPrefab;
                _trackManager.PrepareForRace();
            }
            if (_botManager != null)
            {

            }
            if (_resultsManager != null)
            {

            }
            if (_saveManager != null)
            {

            }
            if (_menuManager != null)
            {
                //_menuManager.SetPlayerSave(PlayerSave);
                _menuManager.SetCarDropdown(PlayerSave.Info.CarID);
                //открывать или закрывать доступ в соответствии с сейвом
            }
            if(_audioManager != null)
            {
                _audioManager.SetVolume(PlayerSave.Info.SoundVolume);
            }
        }

        public void FindManagers()
        {
            _trackManager = null;
            _botManager = null;
            _resultsManager = null;
            _saveManager = null;
            _menuManager = null;
            _audioManager = null;

            _trackManager = FindObjectOfType<TrackManager>();
            _botManager = FindObjectOfType<AI.BotManager>();
            _resultsManager = FindObjectOfType<ResultsManager>();
            _saveManager = FindObjectOfType<SaveManager>();
            _menuManager = FindObjectOfType<Menu.MenuManager>();
            _audioManager = FindObjectOfType<AudioManager>();
        }

        public void StartRace()
        {
            _trackManager.StartRace();
        }

        public void LoadMainMenu()
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenuScene")) return;
            LoadLevel("MainMenuScene");
        }

        public void SaveGame()
        {
            if (_saveManager != null)
            {
                _saveManager.WriteSave(PlayerSave.Info);
            }
            else
            {
                Debug.LogError("Save Manager is not found and PlayerSave can't be written");
            }
        }

        public void SetVolume(float volume)
        {
            PlayerSave.Info.SoundVolume = volume;
            if (_audioManager != null)
            {
                _audioManager.SetVolume(PlayerSave.Info.SoundVolume);
                SaveGame();
            }
        }
    }
}