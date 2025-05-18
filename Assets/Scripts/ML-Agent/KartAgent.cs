using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class KartAgent : Agent
{
    private KartController kartController;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private int nextCheckpointIndex = 0;

    public override void Initialize()
    {
        Checkpoint[] checkpointArray = FindObjectsOfType<Checkpoint>();
        checkpoints = new List<Checkpoint>(checkpointArray);
        checkpoints.Sort((a, b) => a.checkpointIndex.CompareTo(b.checkpointIndex));

        kartController = GetComponent<KartController>();
    }

    public override void OnEpisodeBegin()
    {
        kartController.ResetKart(); 
        nextCheckpointIndex = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    { 
        Vector3 directionToCheckpoint = (checkpoints[nextCheckpointIndex].transform.position - transform.position).normalized;
        sensor.AddObservation(directionToCheckpoint);

        sensor.AddObservation(kartController.GetCurrentSpeed());

        sensor.AddObservation(kartController.steeringInput);
        sensor.AddObservation(kartController.GetDriftingState());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float accelerationInput = 0f,
              steeringInput = 0f;

        bool drift = actions.DiscreteActions[2] == 1;

        switch (actions.DiscreteActions[0])
        {
            case 0:
                accelerationInput = 0f;
                break;
            case 1:
                accelerationInput += 1f;
                break;
            case 2:
                accelerationInput -= 1f;
                break;
        }

        switch (actions.DiscreteActions[1])
        {
            case 0:
                steeringInput = 0f;
                break;
            case 1:
                steeringInput += 1f;
                break;
            case 2:
                steeringInput -= 1f;
                break;
        }

        kartController.SetDrivingParam(accelerationInput, steeringInput, drift);

        //Debug.Log($"Acceleration: {accelerationInput}, Steering: {steeringInput}, Drift: {drift}");

        if (kartController.GetCurrentSpeed() > 0.1f)
            AddReward(0.01f);
        else
            AddReward(-0.01f);

        float distanceToCheckpoint = Vector3.Distance(transform.position, checkpoints[nextCheckpointIndex].transform.position);
        AddReward(1.0f / (1.0f + distanceToCheckpoint)); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        { 
            if (checkpoints[nextCheckpointIndex].gameObject == other.gameObject)
            {
                nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpoints.Count;
                AddReward(1.0f); 
            }
        }
        else if (other.CompareTag("AIWall"))
        {      
            AddReward(-1.0f);
            //EndEpisode(); 
        }


    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        discreteActions[1] = 0;
        discreteActions[2] = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;

        if (Input.GetAxis("Vertical") > 0f)
        {
            discreteActions[0] = 1;
        }
        else if(Input.GetAxis("Vertical") < 0f)
        {
            discreteActions[0] = 2;
        }

        if (Input.GetAxis("Horizontal") > 0f)
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            discreteActions[1] = 2;
        }
    }
}