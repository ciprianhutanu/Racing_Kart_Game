using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        RacerProgress racer = other.GetComponent<RacerProgress>();
        if (racer != null && (!racer.IsCheckpointActivated(checkpointIndex) || (racer.ALL_CHECKPOINTS_FLAG && checkpointIndex == 0)))
        {
            racer.PassCheckpoint(checkpointIndex);
        }
    }
}