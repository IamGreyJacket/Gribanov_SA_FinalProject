using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("Spawn-related"), SerializeField]
        private Transform[] _spawnPoints;
        private int _busySpawnPointCount = 0;

        private void Awake()
        {
            if (_spawnPoints.Length <= 0) Debug.LogError("Spawn points are not set to SpawnManager");
        }

        public void SpawnCar(CarComponent car)
        {
            if (_busySpawnPointCount < _spawnPoints.Length)
            {
                var point = _spawnPoints[_busySpawnPointCount];
                var carClone = Instantiate(car, point.position, point.rotation);
                AudioManager.Self.GetAudioSources();
                AudioManager.Self.SetAudioSources();
                if (carClone.GetComponent<Player.PlayerCarController>() != null)
                {
                    var visuals = carClone.GetComponent<VisualTuningComponent>();
                    visuals.SetSpoilerMeshFromID(GameManager.Self.PlayerSave.Info.SpoilerID);
                    visuals.SetWheelMeshFromID(GameManager.Self.PlayerSave.Info.WheelsID, GameManager.Self.PlayerSave.Info.WheelsScale);
                    var cameraController = carClone.GetComponent<Player.CameraController>();
                    cameraController.SetCar(carClone);
                    cameraController.SetCamera(Camera.main);
                    Camera.main.GetComponent<Player.Dashboard>().SetCar(carClone);
                    Camera.main.GetComponentInChildren<MinimapComponent>().SetCar(carClone);
                    var raceHUD = Camera.main.GetComponent<Player.RaceHUD>();
                    if (raceHUD != null) raceHUD.SetJudge(carClone.GetComponent<Assistants.Judge>());

                }
                else if (carClone.GetComponent<AI.BotCarController>())
                {

                }
                _busySpawnPointCount++;
            }
            else
            {
                Debug.LogWarning("All spawn points are busy");
            }
        }
    }
}