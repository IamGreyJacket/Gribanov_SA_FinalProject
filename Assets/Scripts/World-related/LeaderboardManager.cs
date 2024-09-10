using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Cars
{
    public class LeaderboardManager : MonoBehaviour
    {
        private FileStream _leaderboardStream;
        private StreamWriter _leaderboardWriter;
        private StreamReader _leaderboardReader;
        [SerializeField]
        private string _leaderboardName = $"Leaderboard.txt";
        private string _leaderboardPath;
        [SerializeField]
        private string _leaberboardFolderName = "Leaderboard";
        public static LeaderboardManager Self { get; private set; }

        private void OnValidate()
        {
            if(_leaberboardFolderName == string.Empty || _leaberboardFolderName == "")
            {
                _leaberboardFolderName = "Leaderboard";
                Debug.LogWarning("You have to set a name for a folder, it cannot be empty");
            }
        }

        private void Awake()
        {
            Self = this;
            try
            {
                _leaderboardPath = $"{_leaberboardFolderName}\\{_leaderboardName}";
                _leaderboardStream = new FileStream(_leaderboardPath, FileMode.OpenOrCreate);
                _leaderboardStream.Close();
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\{_leaberboardFolderName}");
                _leaderboardStream = new FileStream(_leaderboardPath, FileMode.OpenOrCreate);
                _leaderboardStream.Close();
            }
        }

        private void Start()
        {

        }
        
        public Dictionary<float, string> GetRecords()
        {
            Dictionary<float, string> result = new Dictionary<float, string>();
            _leaderboardReader = new StreamReader(_leaderboardPath);
            string line;
            while ((line = _leaderboardReader.ReadLine()) != null)
            {
                string[] temp = line.Split();
                if (temp[0] == "[Don\'t_count]")
                {
                    continue;
                }
                else
                {
                    result[float.Parse(temp[0])] = temp[1];
                }
            }
            _leaderboardReader.Dispose();
            return result;
        }

        public string[] GetNames()
        {
            List<string> result = new List<string>();
            _leaderboardReader = new StreamReader(_leaderboardPath);
            string line;
            while ((line = _leaderboardReader.ReadLine()) != null)
            {
                string[] temp = line.Split();
                if (temp[0] == "[Don\'t_count]")
                {
                    continue;
                }
                else
                {
                    result.Add(temp[1]);
                }
            }
            _leaderboardReader.Dispose();
            return result.ToArray();
        }

        public float[] GetTimes()
        {
            List<float> result = new List<float>();
            _leaderboardReader = new StreamReader(_leaderboardPath);
            string line;
            while ((line = _leaderboardReader.ReadLine()) != null)
            {
                string[] temp = line.Split();
                if (temp[0] == "[Don\'t_count]")
                {
                    continue;
                }
                else
                {
                    result.Add(float.Parse(temp[0]));
                }
            }
            _leaderboardReader.Dispose();
            return result.ToArray();
        }

        public void Write(string text)
        {
            _leaderboardWriter = new StreamWriter(_leaderboardPath, true);
            _leaderboardWriter.WriteLine($"[Don't_count] {text}");
            _leaderboardWriter.Dispose();
        }

        public void WriteTime(string[] names, float[] times)
        {
            //"99:99:9 DRIVERNAME"
            _leaderboardWriter = new StreamWriter(_leaderboardPath);
            for (int i = 0; i < times.Length; i++)
            {
                _leaderboardWriter.WriteLine($"{times[i]} {names[i]}");
            }
            _leaderboardWriter.Dispose();
        }

        private void OnDestroy()
        {
            _leaderboardStream.Dispose();
        }
    }
}
