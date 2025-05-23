using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Driving Attributes")]
    public float forwardAcceleration = 20f;
    public float reverseAcceleration = 10f;
    public float groundCheckRadius = 0.5f;
    public float maxForwardSpeed = 30f;
    public float maxReverseSpeed = 15f;
    public float turnSpeed = 50f;
    public float drag = 7f;

    [Header("Drifting")]
    public float driftTurnMultiplier = 2.5f;
    public float driftBoostMultiplier = 1.5f;
    public float driftStaminaBar = 5f;
    public float driftCooldown = 10f;
    public float maxDriftBoost = 10f;
    public float maxDriftAngle = 25f; 
    public float driftSmoothing = 5f;
    public Transform carModel;

    [Header("Ground Layer Checker")]
    public LayerMask groundLayer; 
    public Transform groundCheck;

    private bool driftDirection; // stearingInput > 0 => true else false
 
    private bool isDrifting;
    private bool isGrounded;

    private float currentDriftTime = 0f;
    private float targetDriftAngle = 0f;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float driftTime = 0f;

    [HideInInspector]
    public float accelerationInput = 0f;
    [HideInInspector]
    public float steeringInput = 0f;

    private bool BRAKE_FLAG;
    private bool NOT_ABLE_TO_DRIFT_FLAG = false;

    public Rigidbody rb;

    void Start()
    {
        isDrifting = false;

        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.drag = drag;
    }

    void Update()
    {
        accelerationInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            CheckForDrift();

            if (Input.GetKeyDown(KeyCode.LeftShift) && Mathf.Abs(steeringInput) > 0.1f && currentSpeed > 0 && !NOT_ABLE_TO_DRIFT_FLAG)
            {
                targetDriftAngle = maxDriftAngle * Mathf.Sign(steeringInput);

                StartDrift();
            }

            if ((Input.GetKeyUp(KeyCode.LeftShift) || driftDirection != (steeringInput > 0) || currentSpeed < 0 || NOT_ABLE_TO_DRIFT_FLAG) && isDrifting)
            {
                targetDriftAngle = 0f;

                EndDrift();
            }

            float currentAngle = Mathf.LerpAngle(carModel.localEulerAngles.y, targetDriftAngle, Time.deltaTime * driftSmoothing);
            carModel.localRotation = Quaternion.Euler(0f, currentAngle, 0f);

            HandleMovement(); 
        }

        UpdateWheels();
    }

    void FixedUpdate()
    {
        float adjustedTurnSpeed = isDrifting ? turnSpeed * driftTurnMultiplier : turnSpeed;

        Vector3 velocity = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position - velocity);

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turnAmount = steeringInput * adjustedTurnSpeed * Mathf.Clamp01(currentSpeed / maxForwardSpeed) * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            if (isGrounded)
            {
                rb.MoveRotation(rb.rotation * turnRotation);
            }
        }

    }

    private void StartDrift()
    {
        isDrifting = true;

        driftDirection = steeringInput > 0 ? true : false;

        currentDriftTime = 0f;
    }

    private void EndDrift()
    {
        isDrifting = false;
        float boost = Mathf.Clamp(currentDriftTime, 0f, maxDriftBoost) * driftBoostMultiplier;
        currentSpeed += boost;

        currentDriftTime = 0f;
    }

    private void ResetDriftFlag()
    {
        NOT_ABLE_TO_DRIFT_FLAG = false;
    }

    private void CheckForDrift()
    {
        Debug.Log(driftTime);

        if (isDrifting)
        {
            currentDriftTime += Time.deltaTime;
            driftTime += Time.deltaTime;

            if (driftTime > driftStaminaBar)
            {
                NOT_ABLE_TO_DRIFT_FLAG = true;
                Invoke("ResetDriftFlag", driftCooldown);
                driftTime = 0f;
            }
        }
        else
        {
            driftTime = driftTime > 0 ? driftTime - Time.deltaTime / 10 : 0;
        }
    }

    void HandleMovement()
    {
        if (isDrifting)
        {
            driftTime += Time.deltaTime;
        }

        float acceleration;

        BRAKE_FLAG = false;

        targetSpeed = CalculateTargetSpeed();

        if (BRAKE_FLAG)
        {
            acceleration = reverseAcceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * Time.deltaTime);
        }
        else
        {
            if (targetSpeed == 0)
            {
                acceleration = drag;
            }
            else
            {
                acceleration = targetSpeed > currentSpeed ? forwardAcceleration : reverseAcceleration;
            }

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }

        Debug.Log(currentSpeed + " " + accelerationInput);
    }

    private float CalculateTargetSpeed()
    {
        float _targetSpeed = 0f;

        if (accelerationInput > 0)
        {
            _targetSpeed = maxForwardSpeed * accelerationInput;
        }
        else if (accelerationInput < 0)
        {
            if (currentSpeed > 0)
            {
                _targetSpeed = 0f;
                BRAKE_FLAG = true;
            }
            else
            {
                _targetSpeed = maxReverseSpeed * accelerationInput;
            }
        }
        else
        {
            _targetSpeed = 0f;
        }

        return _targetSpeed;
    }

    void UpdateWheels()
    {
        WheelController.kartSpeed = currentSpeed;
        WheelController.steeringInput = steeringInput;
    }
}
