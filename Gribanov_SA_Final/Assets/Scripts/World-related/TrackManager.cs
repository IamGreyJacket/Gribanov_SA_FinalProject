using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

using Racer.Managers.Assistants;

namespace Racer.Managers
{
    public class TrackManager : MonoBehaviour
    {
        [SerializeField]
        private RaceType _raceType;
        public RaceType RaceType 
        {
            get => _raceType;
            set
            {
                _raceType = value;
            }
        }

        [SerializeField]
        private Checkpoint[] _checkpoints;
        [SerializeField]
        private FinishLine _finishLine;

        private byte _finishCarsCounter;

        [SerializeField, Min(1)]
        private byte _laps = 3;
        [SerializeField, Tooltip("Time to beat (in seconds)")]
        private float _timeToBeat;

        public byte Laps
        {
            get => _laps;
            set
            {
                _laps = value;
            }
        }

        public float TimeToBeat
        {
            get => _timeToBeat;
            set
            {
                _timeToBeat = value;
            }
        }

        [Space, SerializeField]
        private GameObject _raceInfoWindow;
        [SerializeField]
        private TextMeshProUGUI _raceInfoText;
        [SerializeField]
        private TextMeshProUGUI _countdownText;
        [SerializeField]
        private int _countdown = 3;

        private Judge[] _judges; //Содержит судей, которым будет раздаваться информация о гонке (круги, чекпойнты и т.д.)

        public int OpponentsCount => _judges.Length - 1;

        private void Awake()
        {
            if(_raceInfoText == null || _countdownText == null || _raceInfoWindow == null)
            {
                Debug.LogError("Not all UI Elements are set for Track Manager in the inspector");
            }
            GetJudges();
            if (_raceType != RaceType.Freeroam)
            {
                _finishLine.OnRacerFinishRaceEvent += OnRacerFinish;
                _finishLine.OnRacerFinishLapEvent += SetJudge;
            }
        }

        private void OnDestroy()
        {
            if (_raceType != RaceType.Freeroam)
            {
                _finishLine.OnRacerFinishLapEvent -= SetJudge;
                _finishLine.OnRacerFinishRaceEvent -= OnRacerFinish;
            }
        }

        private void Start()
        {
            SetJudges();
            string text = $" Race Type: {RaceType}\nLaps: {Laps}\n";
            switch (RaceType)
            {
                case RaceType.TimeAttack:
                    var currentTime = TimeToBeat;
                    TimeSpan time = TimeSpan.FromSeconds(currentTime);
                    int minutes = (int)time.TotalMinutes;
                    if (minutes > 99) minutes = 99;
                    text += $"Time to beat: {minutes}:{time.ToString(@"ss\:f")}\n";
                    break;
                case RaceType.Circuit:
                    text += $"Opponents: {OpponentsCount}\n";
                    break;
            }
            text += $"If you want to go back to Main Menu, press Esc";
            _raceInfoText.text = text;
            _raceInfoWindow.SetActive(true);
        }

        public void StartCountdown()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            _countdownText.gameObject.SetActive(true);
            while (_countdown > 0)
            {
                _countdownText.text = _countdown.ToString();
                //_animator.Play("CountdownAnimation");
                yield return new WaitForSeconds(.9f);
                _countdown--;
            }
            _countdownText.text = "START!";
            StartRace();
            //_animator.Play("StartFinishAnimation");
            yield return new WaitForSeconds(2f);
            _countdownText.gameObject.SetActive(false);
            var temp = _countdownText.color;
            temp.a = 1;
            _countdownText.color = temp;
        }
        //Копировать все отсюда в RaceHUD скрипт

        public void StartRace()
        {
            foreach(var judge in _judges)
            {
                judge.StartRace();
            }
        }

        private void OnRacerFinish(Judge judge)
        {
            judge.EndRace();
            _finishCarsCounter++;
            if(judge.gameObject.GetComponent<Player.PlayerCarController>() != null)
            {
                GameManager.Self.ResultsManager.ShowResults(_finishCarsCounter, _judges.Length, _raceType, judge);
            }
            Debug.Log($"Car {judge.name} finished {_finishCarsCounter} / {_judges.Length}");
        }

        private void GetJudges()
        {
            _judges = FindObjectsOfType<Judge>();
        }

        public void SetJudges()
        {
            switch (_raceType) 
            {
                case RaceType.TimeAttack: 
                    for (int i = 0; i < _judges.Length; i++)
                    {
                        _judges[i].SetRaceParams(_checkpoints, Laps, _timeToBeat);
                    }
                    break;

                case RaceType.Circuit:
                    for (int i = 0; i < _judges.Length; i++)
                    {
                        _judges[i].SetRaceParams(_checkpoints, Laps, 0);
                    }
                    break;
            }
        }

        private void SetJudge(Judge judge)
        {
            judge.SetRaceParams(_checkpoints, Laps, _timeToBeat);
        }

    }

    [Serializable]
    public enum RaceType
    {
        Freeroam,
        Circuit,
        TimeAttack,
    }
}