using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Racer.Managers;

namespace Racer.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _startScreen;
        [SerializeField]
        private GameObject _mainMenuPanel;
        [SerializeField]
        private Transform _mainMenuCameraPosition;
        [SerializeField, Tooltip("A place, where car would spawn for player to see it and how it's tuned")]
        private Transform _carSpawnPosition;
        [SerializeField]
        private GameObject _notificationPanel;
        [SerializeField]
        private TextMeshProUGUI _notificationText;

        [Space, Header("Settings-related"), SerializeField]
        private GameObject _settingsPanel;
        [SerializeField]
        private Slider _volumeSlider;

        [Space, Header("Race-related"), SerializeField]
        private GameObject _raceMenuPanel;
        [SerializeField]
        private GameObject _circuitParametersPanel;
        [SerializeField]
        private TMP_Dropdown _carChoiceDropdown;

        [Space, Header("Tuning-related"), SerializeField]
        private TuningMenuComponent _tuningComponent;
        [SerializeField]
        private GameObject _tuningPanel;
        [SerializeField]
        private TMP_Dropdown _carChoiceTuningDropdown;
        [SerializeField]
        private Transform _tuningCameraPosition;
        [SerializeField]
        private Transform _spoilerCameraPosition;
        //private VisualTuningComponent _currentTuningComponent;
        //public VisualTuningComponent CurrentTuningComponent => _currentTuningComponent;

        public Player.PlayerSaveInfo PlayerSave => Managers.GameManager.Self.PlayerSave;

        //public void SetPlayerSave(Player.PlayerSaveSO playerSave) => _playerSave = playerSave;

        //вставить сюда либо bool либо сейв игрока

        private bool _isStartScreen = true;

        private void Start()
        {
            _volumeSlider.value = Managers.GameManager.Self.PlayerSave.SoundVolume;
            StartCoroutine(StartScreen());
        }

        public void OnChooseCar(int carID)
        {
            Managers.GameManager.Self.SetPlayerCar(carID);
        }

        public void SetCarDropdown(int carID)
        {
            _carChoiceDropdown.value = carID;
            OnChooseCar(carID);
        }

        private IEnumerator StartScreen()
        {
            _isStartScreen = true;
            DisableAllWindows();
            _startScreen.SetActive(true);
            while (_isStartScreen)
            {
                if (Input.anyKey)
                {
                    _isStartScreen = false;
                    Debug.Log("Player pressed some key");
                }
                yield return null;
            }
            ShowMainMenu();
            yield return null;
        }

        public void DisableAllWindows()
        {
            _tuningPanel.SetActive(false);
            _notificationPanel.SetActive(false);
            _circuitParametersPanel.SetActive(false);
            _raceMenuPanel.SetActive(false);
            _mainMenuPanel.SetActive(false);
            _settingsPanel.SetActive(false);
            _startScreen.SetActive(false);
            Camera.main.transform.position = _mainMenuCameraPosition.position;
            Camera.main.transform.rotation = _mainMenuCameraPosition.rotation;
        }

        public void ShowNotification(string text)
        {
            _notificationText.text = text;
            _notificationPanel.SetActive(true);
        }

        public void ShowMainMenu()
        {
            DisableAllWindows();
            _mainMenuPanel.SetActive(true);
        }

        public void ShowSettings()
        {
            DisableAllWindows();
            _settingsPanel.SetActive(true);
        }

        public void ShowRaces()
        {
            DisableAllWindows();
            SetCarDropdown(PlayerSave.CarID);
            _raceMenuPanel.SetActive(true);
            //Отключает главное меню и показывает варианты гонок в виде кнопок
        }

        public void ShowTuning()
        {
            if (PlayerSave.TuningIsOpen)
            {
                DisableAllWindows();
                _carChoiceTuningDropdown.value = 0;
                _tuningPanel.SetActive(true);
                Camera.main.transform.position = _tuningCameraPosition.position;
                Camera.main.transform.rotation = _tuningCameraPosition.rotation;
            }
            else ShowNotification($"You can't tune your car yet. First you need to complete Freeroam");
        }

        public void ShowCircuitParameters()
        {
            if (PlayerSave.CircuitIsOpen)
            {
                DisableAllWindows();
                _circuitParametersPanel.SetActive(true);
            }
            else
            {
                string text = "This race is not availabe for you yet. First you need to complete Time Attack";
                ShowNotification(text);
            }
        }

        public void LoadLevel(string sceneName)
        {
            GameManager.Self.LoadLevel(sceneName);
        }

        #region LoadingLevelsFunctions

        public void LoadFreeroam()
        {
            
            LoadLevel("FreeroamScene");
            PlayerSave.TimeAttackIsOpen = true;
            PlayerSave.TuningIsOpen = true;
            Managers.GameManager.Self.SaveGame();
            //just loads FreeroamScene
        }

        public void LoadTimeAttack()
        {
            if (PlayerSave.TimeAttackIsOpen) LoadLevel("TimeAttack");
            else
            {
                string text = "This race is not availabe for you yet. First you need to complete Freeroam";
                ShowNotification(text);
            }
            //Loads TimeAttackScene with default time-to-beat
        }

        public void LoadCircuit()
        {
            if(PlayerSave.CircuitIsOpen) LoadLevel("RacetrackScene");
            else 
            {
                string text = "This race is not availabe for you yet. First you need to complete TimeAttack";
                ShowNotification(text);
            }
            //Loads CircuitRaceScene withdefault amount of bots and default quantity of laps
        }

        #endregion

        public void ExitGame()
        {
            Debug.Log("Quitting the game");
            //Save all data, such as progress, money, car setup, etc. into ScriptableObject
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}