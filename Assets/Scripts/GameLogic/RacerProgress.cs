using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RacerProgress : MonoBehaviour
{
    public string racerName = "Player";
    public int totalCheckpoints = 0;

    private int currentLap = 0;
    private int nextCheckpointIndex = 0;
    private bool raceCompleted = false;
    public bool ALL_CHECKPOINTS_FLAG = false;

    private float lapStartTime;
    private List<float> checkpointTimes = new List<float>();
    private List<float> lapTimes = new List<float>();

    private List<bool> checkpointActivation = new List<bool>();

    void Start()
    {
        lapStartTime = LapManager.Instance.GetGlobalRaceTime();

        for(int i = 0; i < totalCheckpoints; i++)
        {
            checkpointActivation.Add(false);
        }
    }

    public void PassCheckpoint(int checkpointIndex)
    {
        if (raceCompleted) return;

        checkpointActivation[checkpointIndex] = true;

        float currentTime = LapManager.Instance.GetGlobalRaceTime();

        float checkpointTime = currentTime - lapStartTime;
        checkpointTimes.Add(checkpointTime);

        nextCheckpointIndex++;

        //Debug.Log($"{racerName} passed checkpoint {checkpointIndex}! Time: {checkpointTime:F2}s");
        
        LapManager.Instance.RegisterCheckpoint(this);

        if (ALL_CHECKPOINTS_FLAG && checkpointIndex == 0)
        {
            resetCheckpoints();
            ALL_CHECKPOINTS_FLAG = false;

            nextCheckpointIndex = 1;
            currentLap++;

            float lapTime = currentTime - lapStartTime;
            lapTimes.Add(lapTime);
            lapStartTime = currentTime;

            KartDataWrapper dataWrapper = new KartDataWrapper
            {
                kartDataList = GetComponent<KartController>().kartData
            };

            //System.IO.File.WriteAllText(
            //    "C:\\Users\\euseb\\Documents\\Unity\\Racing_Kart_Game\\ResultsData\\lap_data.json",
            //    JsonUtility.ToJson(dataWrapper)
            //);

            Debug.Log($"Saved {dataWrapper.kartDataList.Count} entries to lap_data.json");

            //Debug.Log($"{racerName} completed lap {currentLap}! Lap Time: {lapTime:F2}s");

            if (LapManager.Instance.IsRaceFinished(currentLap))
            {
                raceCompleted = true;
                float totalRaceTime = currentTime;
                Debug.Log($"{racerName} finished the race! Total Time: {totalRaceTime:F2}s");
                LapManager.Instance.RegisterFinish(this);
            }
        }
        else if(nextCheckpointIndex == totalCheckpoints)
        {
            ALL_CHECKPOINTS_FLAG = true;
        }
    }

    public float GetCurrentRaceTime()
    {
        return LapManager.Instance.GetGlobalRaceTime();
    }

    public int GetCurrentLap()
    {
        return currentLap;
    }

    public int GetNextCheckpointIndex()
    {
        return nextCheckpointIndex;
    }

    public List<float> GetCheckpointTimes()
    {
        return new List<float>(checkpointTimes);
    }

    public List<float> GetLapTimes()
    {
        return new List<float>(lapTimes);
    }

    public string GetLapFormated()
    {
        return currentLap + "/" + LapManager.totalLaps;
    }

    public bool IsCheckpointActivated(int checkpointIndex)
    {
        try
        {
            return checkpointActivation[checkpointIndex];
        } catch(IndexOutOfRangeException e)
        {
            Debug.LogException(e);
            return true;
        }
        
    }

    private void resetCheckpoints()
    {
        for(int i = 1; i < totalCheckpoints; i++)
        {
            checkpointActivation[i] = false;
        }
    }
}