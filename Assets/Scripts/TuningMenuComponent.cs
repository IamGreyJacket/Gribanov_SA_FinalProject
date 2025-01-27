using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Racer.Menu
{
    public class TuningMenuComponent : MonoBehaviour
    {
        private VisualTuningComponent _currentVisualComponent;
        public VisualTuningComponent CurrentVisualComponent => _currentVisualComponent;
        [SerializeField]
        private TMP_Dropdown _wheelsDropdown;
        [SerializeField]
        private TMP_Dropdown _spoilerDropdown;
        [SerializeField]
        private Transform _tuningSpawnPoint;

        [SerializeField]
        private CarComponent _ladaPrefab;
        [SerializeField]
        private CarComponent _uazPrefab;
        [SerializeField]
        private CarComponent _gazPrefab;

        private Vector3 _wheelsScale = new Vector3(1f, 1f, 1f);

        private bool _isSaved = true;
        public bool IsSaved => _isSaved;

        public void OnChooseCar(int carID)
        {
            if (_currentVisualComponent != null) Destroy(_currentVisualComponent.gameObject);
            _spoilerDropdown.options = new List<TMP_Dropdown.OptionData>();
            _wheelsDropdown.options = new List<TMP_Dropdown.OptionData>();
            Managers.GameManager.Self.SetPlayerCar(carID);
            var save = Managers.GameManager.Self.PlayerSave;
            CarComponent car = null;
            switch (carID)
            {
                case 0:
                    car = null;
                    break;
                case 1:
                    car = _ladaPrefab;
                    _wheelsScale = new Vector3(.7f, .7f, .7f);
                    break;
                case 2:
                    car = _uazPrefab;
                    _wheelsScale = new Vector3(.9f, .9f, .9f);
                    break;
                case 3:
                    car = _gazPrefab;
                    _wheelsScale = new Vector3(.8f, .8f, .8f);
                    break;
            }
            var point = _tuningSpawnPoint;
            CarComponent carClone;
            if (car != null)
            {
                carClone = Instantiate(car, point.position, point.rotation);

                //Managers.AudioManager.Self.GetAudioSources();
                //Managers.AudioManager.Self.SetAudioSources();

                _currentVisualComponent = carClone.GetComponent<VisualTuningComponent>();
                var options = _wheelsDropdown.options;
                for (int i = 0; i < _currentVisualComponent.LeftWheelMeshes.Length; i++)
                {
                    if (i == 0) options.Add(new TMP_Dropdown.OptionData($"Standart wheels"));
                    else options.Add(new TMP_Dropdown.OptionData($"Wheels {i}"));
                }
                _wheelsDropdown.options = options;
                _wheelsDropdown.value = save.WheelsID;
                _currentVisualComponent.SetWheelMeshFromID(save.WheelsID, _wheelsScale);
                options = _spoilerDropdown.options;
                for (int i = 0; i < _currentVisualComponent.SpoilerMeshes.Length; i++)
                {
                    if (i == 0) options.Add(new TMP_Dropdown.OptionData($"None"));
                    else options.Add(new TMP_Dropdown.OptionData($"Spoiler {i}"));
                }
                _spoilerDropdown.options = options;
                _spoilerDropdown.value = save.WheelsID;
                _currentVisualComponent.SetSpoilerMeshFromID(save.SpoilerID);
            }
        }

        public void OnChooseWheels(int wheelsID)
        {
            _currentVisualComponent.SetWheelMeshFromID(wheelsID, _wheelsScale);
            _isSaved = false;
        }

        public void OnChooseSpoiler(int spoilerID)
        {
            _currentVisualComponent.SetSpoilerMeshFromID(spoilerID);
            _isSaved = false;
        }

        public void OnTuningEnd()
        {
            if (_currentVisualComponent != null)
            {
                var gameManager = Managers.GameManager.Self;
                if (gameManager == null)
                {
                    Debug.LogError("Game Manager isn't present, so changes cannot be saved for the next scene.");
                    return;
                }
                if (_isSaved == false)
                {
                    if (gameManager.PlayerSave.WheelsID == 0 || gameManager.PlayerSave.WheelsID >= _currentVisualComponent.LeftWheelMeshes.Length)
                    {
                        _currentVisualComponent.SetWheelMeshFromID(0, new Vector3(100, 100, 100));
                        gameManager.PlayerSave.WheelsScale = new Vector3(100, 100, 100);
                    }
                    else
                    {
                        _currentVisualComponent.SetWheelMeshFromID(gameManager.PlayerSave.WheelsID, _wheelsScale);
                        gameManager.PlayerSave.WheelsScale = _wheelsScale;
                    }
                }
                if (gameManager.PlayerSave.SpoilerID >= _currentVisualComponent.SpoilerMeshes.Length) _currentVisualComponent.SetSpoilerMeshFromID(0);
                else _currentVisualComponent.SetSpoilerMeshFromID(gameManager.PlayerSave.SpoilerID);

                SaveChanges();
                _isSaved = true;
                Debug.Log($"WheelsID: {gameManager.PlayerSave.WheelsID} |" +
                    $" WheelsScale: {gameManager.PlayerSave.WheelsScale} |" +
                    $" SpoilerID: {gameManager.PlayerSave.SpoilerID}");
                _spoilerDropdown.options = new List<TMP_Dropdown.OptionData>();
                _wheelsDropdown.options = new List<TMP_Dropdown.OptionData>();
                Destroy(_currentVisualComponent.gameObject);
            }
        }

        public void SaveChanges()
        {
            var gameManager = Managers.GameManager.Self;
            if (gameManager == null)
            {
                Debug.LogError("Game Manager isn't present, so changes cannot be saved for the next scene.");
                return;
            }
            //if(доступно)
            gameManager.PlayerSave.WheelsID = _currentVisualComponent.WheelsCurrentID;
            if (gameManager.PlayerSave.WheelsID == 0) gameManager.PlayerSave.WheelsScale = new Vector3(100, 100, 100);
            else gameManager.PlayerSave.WheelsScale = _wheelsScale;
            //if(доступно)
            gameManager.PlayerSave.SpoilerID = _currentVisualComponent.SpoilerCurrentID;
            gameManager.SaveGame();
            _isSaved = true;
            Debug.Log($"Current WheelID: {_currentVisualComponent.WheelsCurrentID} |" +
                $" WheelsScale: {gameManager.PlayerSave.WheelsScale} |" +
                $" Current SpoilerID: {_currentVisualComponent.SpoilerCurrentID}");
        }
    }
}