using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas gameUI;
    public Canvas pauseMenu;

    private bool canTogglePause = true;
    private float transitionDuration = 1f;
    private float pauseCooldown = 1f; 
    private KartController kartController;

    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Vector3 pauseCameraPosition = new Vector3(0, 200, 0);
    private Quaternion pauseCameraRotation = Quaternion.Euler(90f, 0f, 0f);

    private bool PAUSED_FLAG = false;

    private void Start()
    {
        kartController = GetComponent<KartController>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main; 
        }

        originalCameraPosition = mainCamera.transform.position;
        originalCameraRotation = mainCamera.transform.rotation;
    }

    private void Update()
    {
        float accelerationInput = Input.GetAxis("Vertical"); 
        float steeringInput = Input.GetAxis("Horizontal");  
        bool drift = Input.GetKey(KeyCode.LeftShift);       

        kartController.SetDrivingParam(accelerationInput, steeringInput, drift);

        if (Input.GetKeyDown(KeyCode.Escape) && canTogglePause)
        {
            if (PAUSED_FLAG)
            {
                StartCoroutine(ResumeGame());
            }
            else
            {
                originalCameraPosition = mainCamera.transform.position;
                originalCameraRotation = mainCamera.transform.rotation;

                StartCoroutine(PauseGame());
            }

        }
    }

    private IEnumerator PauseGame()
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        gameUI.gameObject.SetActive(false);
        AudioListener.volume = 0f;

        Time.timeScale = 0f;

        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, pauseCameraPosition, elapsedTime / transitionDuration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, pauseCameraRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = pauseCameraPosition;
        mainCamera.transform.rotation = pauseCameraRotation;
        PAUSED_FLAG = true;

        pauseMenu.gameObject.SetActive(true);
    }

    private IEnumerator ResumeGame()
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        pauseMenu.gameObject.SetActive(false);

        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPos, originalCameraPosition, elapsedTime / transitionDuration);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, originalCameraRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        mainCamera.transform.rotation = originalCameraRotation;

        yield return new WaitForSecondsRealtime(pauseCooldown);

        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        PAUSED_FLAG = false;

        gameUI.gameObject.SetActive(true);
    }

    public void ResumeRace()
    {
        StartCoroutine(ResumeGame());
    }
}
