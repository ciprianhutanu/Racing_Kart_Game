using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public enum WheelType
    {
        FRONT,
        REAR
    }

    public WheelType wheelPosition;
    public Transform wheelMesh; 
    public float wheelRadius = 0.08f;
    public float maxSteeringAngle = 30f;

    public KartController kartController;

    private ParticleSystem driftSmoke;

    private bool startedDrifting = false;

    private float kartSpeed;
    private float steeringInput;

    void Start()
    {
        kartSpeed = 0;
        steeringInput = 1;

        driftSmoke = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        kartSpeed = kartController.GetCurrentSpeed();
        steeringInput = kartController.steeringInput;

        RotateWheel();
        if (wheelPosition == WheelType.FRONT)
        {
            SteerWheel();
        }

        if(wheelPosition == WheelType.REAR)
        {
            if (kartController.GetDriftingState() && !startedDrifting)
            {
                driftSmoke.Play();
                startedDrifting = true;
            }
            else if (!kartController.GetDriftingState() && startedDrifting)
            {
                startedDrifting = false;
                driftSmoke.Stop();
            }
        }
    }

    void RotateWheel()
    {
        float rollingAngle = (kartSpeed * Time.deltaTime) / (2 * Mathf.PI * wheelRadius) * 360f;
        wheelMesh.Rotate(0f, rollingAngle, 0f, Space.Self);
    }

    void SteerWheel()
    {
        float steeringAngle = maxSteeringAngle * steeringInput;
        wheelMesh.localRotation = Quaternion.Euler(0f, steeringAngle, 90);
    }
}
