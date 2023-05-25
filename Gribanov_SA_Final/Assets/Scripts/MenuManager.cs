using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace Racer.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _startScreen;
        [SerializeField]
        private GameObject _mainMenuPanel;
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

        private Player.PlayerSave _playerSave;
        public void SetPlayerSave(Player.PlayerSave playerSave) => _playerSave = playerSave;

        //вставить сюда либо bool либо сейв игрока

        private bool _isStartScreen = true;

        private void Start()
        {
            _volumeSlider.value = Managers.GameManager.Self.PlayerSave.SoundVolume;
            StartCoroutine(StartScreen());
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
            _notificationPanel.SetActive(false);
            _circuitParametersPanel.SetActive(false);
            _raceMenuPanel.SetActive(false);
            _mainMenuPanel.SetActive(false);
            _settingsPanel.SetActive(false);
            _startScreen.SetActive(false);
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
            _raceMenuPanel.SetActive(true);
            //Отключает главное меню и показывает варианты гонок в виде кнопок
        }

        public void ShowCircuitParameters()
        {
            if (_playerSave.CircuitIsOpen)
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

        public async void LoadLevel(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;
            while(scene.progress < .9f)
            {
                await Task.Delay(100);
            }
            scene.allowSceneActivation = true;
        }

        #region LoadingLevelsFunctions

        public void LoadFreeroam()
        {
            
            LoadLevel("FreeroamScene");
            _playerSave.TimeAttackIsOpen = true;
            //just loads FreeroamScene
        }

        public void LoadTimeAttack()
        {
            if (_playerSave.TimeAttackIsOpen) LoadLevel("TimeAttack");
            else
            {
                string text = "This race is not availabe for you yet. First you need to complete Freeroam";
                ShowNotification(text);
            }
            //Loads TimeAttackScene with default time-to-beat
        }

        public void LoadCircuit()
        {
            if(_playerSave.CircuitIsOpen) LoadLevel("RacetrackScene");
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