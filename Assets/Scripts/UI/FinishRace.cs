using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishRace : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas gameUI;
    public Canvas finishMenu;
    public Text resultText;

    private float transitionDuration = 1f;
    private Vector3 pauseCameraPosition = new Vector3(0, 200, 0);
    private Quaternion pauseCameraRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Start()
    { 
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public IEnumerator FinishRacePlayer(int positionResult)
    {
        float elapsedTime = 0f;
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        string result = "You finished ";

        result += positionResult == 0 ? "first!" : "second!";
        resultText.text = result;

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

        finishMenu.gameObject.SetActive(true);
    }
}
