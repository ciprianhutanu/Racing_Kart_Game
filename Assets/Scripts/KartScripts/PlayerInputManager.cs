using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private KartController kartController;

    private void Start()
    {
        kartController = GetComponent<KartController>();
    }

    private void Update()
    {
        float accelerationInput = Input.GetAxis("Vertical"); 
        float steeringInput = Input.GetAxis("Horizontal");  
        bool drift = Input.GetKey(KeyCode.LeftShift);       

        kartController.SetDrivingParam(accelerationInput, steeringInput, drift);
    }
}
