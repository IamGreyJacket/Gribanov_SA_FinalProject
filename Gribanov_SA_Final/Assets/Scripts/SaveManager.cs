using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO;

namespace Racer.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Self { get; private set; }

        private FileStream _playerSaveStream;
        private StreamWriter _playerSaveWriter;
        private StreamReader _playerSaveReader;

        private string _playerSavePath = "Save\\PlayerSave.json";
        private string _playerSaveName = "PlayerSave.json";
        private string _playerSaveFolderName = "Save";

        private void Awake()
        {
            if (Self == null)
            {
                Self = this;
                DontDestroyOnLoad(this);

                try
                {
                    _playerSavePath = $"{_playerSaveFolderName}\\{_playerSaveName}";
                    _playerSaveStream = new FileStream(_playerSavePath, FileMode.OpenOrCreate);
                    _playerSaveStream.Close();
                }
                catch (DirectoryNotFoundException)
                {
                    Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\{_playerSaveFolderName}");
                    _playerSaveStream = new FileStream(_playerSavePath, FileMode.OpenOrCreate);
                    _playerSaveStream.Close();
                }
            }
            else
            {
                Destroy(this);
            }
            //JsonUtility.FromJson<Player.PlayerSave>();
        }

        public void WriteSave(Player.PlayerSaveInfo playerSave)
        {
            _playerSaveWriter = new StreamWriter(_playerSavePath);
            var objectInfo = JsonUtility.ToJson(playerSave);
            Debug.Log(objectInfo);
            _playerSaveWriter.WriteLine(objectInfo);
            _playerSaveWriter.Dispose();
        }

        public string GetSave()
        {
            _playerSaveReader = new StreamReader(_playerSavePath);
            var objectInfo = _playerSaveReader.ReadToEnd();
            Debug.Log(objectInfo);
            if (objectInfo != "" && objectInfo != null)
            {
                _playerSaveReader.Dispose();
                return objectInfo;
            }
            else
            {
                _playerSaveReader.Dispose();
                return null;
            }
        }

        private void OnDestroy()
        {
            if(_playerSaveStream != null) _playerSaveStream.Dispose();
        }
    }
}