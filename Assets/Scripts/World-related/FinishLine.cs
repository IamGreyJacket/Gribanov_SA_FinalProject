using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racer.Managers.Assistants
{
    public class FinishLine : MonoBehaviour
    {
        //this is for ResultsManager, TrackManager, GameManager, etc.
        public event Action<Judge> OnRacerFinishRaceEvent;
        public event Action<Judge> OnRacerFinishLapEvent;

        private void OnTriggerEnter(Collider other)
        {
            var judge = other.GetComponentInParent<Judge>();
            if (judge == null) return;
            if (!judge.GetCheckpointsStatus())
            {
                Debug.Log("Lap did not count");
                return;
            }
            judge.OnFinishLap();
            OnRacerFinishLapEvent?.Invoke(judge);
            Debug.Log("Lap counted");
            if (judge.GetLapsDone == judge.TotalLaps || judge.LapTimes[judge.GetLapsDone - 1] < judge.TimeToBeat)
            {
                OnRacerFinishRaceEvent?.Invoke(judge);
                Debug.Log($"{judge.GetLapsDone} / {judge.TotalLaps}. {judge.LapTimes[judge.GetLapsDone - 1]} / {judge.TimeToBeat}");
            }
        }
    }
}