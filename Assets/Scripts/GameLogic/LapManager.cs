using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    public static LapManager Instance;

    public static int totalLaps = 3;

    private float raceStartTime;
    private List<(RacerProgress racer, float totalTime)> leaderboard = new List<(RacerProgress, float)>();

    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        raceStartTime = Time.time;

        Checkpoint[] checkpointArray = FindObjectsOfType<Checkpoint>();
        checkpoints = new List<Checkpoint>(checkpointArray);
        checkpoints.Sort((a, b) => a.checkpointIndex.CompareTo(b.checkpointIndex)); 
    }

    public bool IsRaceFinished(int currentLap)
    {
        return currentLap >= totalLaps;
    }

    public void RegisterCheckpoint(RacerProgress racer)
    {
        UpdateLeaderboard(racer);
    }

    public void RegisterFinish(RacerProgress racer)
    {
        this.UpdateLeaderboard(racer);

        Debug.Log($"Finish Line Update: {GetLeaderboard()}");
    }

    private void UpdateLeaderboard(RacerProgress racer)
    {
        if (!leaderboard.Exists(entry => entry.racer == racer))
        {
            leaderboard.Add((racer, 0f));
        }

        leaderboard.Sort((a, b) =>
        {
            int lapCompare = b.racer.GetCurrentLap().CompareTo(a.racer.GetCurrentLap());
            if (lapCompare != 0) return lapCompare;

            int checkpointCompare = b.racer.GetNextCheckpointIndex().CompareTo(a.racer.GetNextCheckpointIndex());
            if (checkpointCompare != 0) return checkpointCompare;

            return a.racer.GetCurrentRaceTime().CompareTo(b.racer.GetCurrentRaceTime());
        });
    }

    public string GetLeaderboard()
    {
        string result = "";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            result += $"{i + 1}. {leaderboard[i].racer.racerName}\n";
        }
        return result;
    }


    public float GetGlobalRaceTime()
    {
        return Time.time - raceStartTime;
    }
}
