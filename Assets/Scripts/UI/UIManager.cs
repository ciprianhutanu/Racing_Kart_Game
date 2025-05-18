using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text speedText;
    public Text lapCounter;
    public Text leaderboardText;
    
    public Slider driftBar;

    [Header("Kart References")]
    public LapManager lapManager;
    public RacerProgress racerProgress;
    public KartController kartController;

    private void Start()
    {
        if (kartController == null)
        {
            Debug.LogError("KartController reference not set in UIManager.");
        }

        if (lapManager == null)
        {
            Debug.LogError("LapManager reference not set in UIManager.");
        }
    }

    private void Update()
    {
        if (kartController == null || lapManager == null) return;

        speedText.text = $"{Mathf.Round(kartController.GetCurrentSpeed())} km/h";

        if (!kartController.GetNATDFlag())
        {
            if (kartController.GetDriftTime() > 0)
            {
                driftBar.value = kartController.GetDriftTime() / kartController.driftStaminaBar;
            }
            else
            {
                driftBar.value = 0;
            }
        }
        else
        {
            driftBar.value = 1;
        }

        lapCounter.text = racerProgress.GetLapFormated();

        leaderboardText.text = lapManager.GetLeaderboard();
    }
}
