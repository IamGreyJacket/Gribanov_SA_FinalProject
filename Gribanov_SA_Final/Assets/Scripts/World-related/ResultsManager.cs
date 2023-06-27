using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Racer.Managers
{
    public class ResultsManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _resultsWindow;
        [SerializeField]
        private TextMeshProUGUI _resultsText;

        private void Start()
        {
            _resultsWindow.SetActive(false);
        }

        public void ShowResults(int position, int racersCount, RaceType raceType, Assistants.Judge judge)
        {
            string text = "";
            judge.LapTimes.Sort();
            var bestTime = judge.LapTimes[0];
            TimeSpan time = TimeSpan.FromSeconds(bestTime);
            int minutes = (int)time.TotalMinutes;
            if (minutes > 99) minutes = 99;
            string bestTimeText = $"{ minutes}:{ time.ToString(@"ss\:f")}";
            switch (raceType)
            {
                case RaceType.TimeAttack:
                    if(judge.LapTimes[0] < judge.TimeToBeat)
                    {
                        text += "Congratulations! You have beaten the target time!";
                        //побил время
                        GameManager.Self.PlayerSave.Info.CircuitIsOpen = true;//todo
                    }
                    else
                    {
                        text += "Too bad. You didn't beat the target time.";
                        //не побил время
                    }
                    bestTime = judge.TimeToBeat;
                    time = TimeSpan.FromSeconds(bestTime);
                    minutes = (int)time.TotalMinutes;
                    if (minutes > 99) minutes = 99;
                    text += $"\nTime to beat was: { minutes}:{ time.ToString(@"ss\:f")}";
                    break;
                case RaceType.Circuit:
                    if(position == 1)
                    {
                        text += "Congratualtions! You've won!";
                        //победил в гонке
                    }
                    else
                    {
                        text += "Too bad. You didn't win.";
                        //проиграл в гонке
                    }
                    text += $"\nYour position is {position} / {racersCount}";
                    break;
            }
            //
            text += $"\nYour best time is: {bestTimeText}";
            text += $"\nPress Escape to go to main menu";
            _resultsText.text = text;
            _resultsWindow.SetActive(true);
        }

    }
}