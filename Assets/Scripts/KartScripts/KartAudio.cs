using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartAudio : MonoBehaviour
{
    public AudioSource idleAudioSource;
    public KartController kart;

    [Header("Audio Settings")]
    public float maxSpeed = 25f; 

    void Update()
    {
        if (kart == null) return;

        float speed = kart.GetCurrentSpeed();
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        float pitch = 1.0f + normalizedSpeed;
        idleAudioSource.pitch = pitch;
    }
}
