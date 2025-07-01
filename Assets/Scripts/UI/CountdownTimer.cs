using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public Text countdownText;
    public Image backgroundImage;
    public Canvas gameUI;
    public int countdownTime = 3; 

    private float rotationAngle = 45f;

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        Time.timeScale = 0f; 
        float realTimeCountdown = countdownTime;

        Quaternion startRotation = backgroundImage.transform.rotation;

        while (realTimeCountdown > 0)
        {
            countdownText.text = Mathf.CeilToInt(realTimeCountdown).ToString();

            Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, rotationAngle);
            float elapsedTime = 0f;
            float rotationDuration = 0.2f; 

            while (elapsedTime < rotationDuration)
            {
                backgroundImage.transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.unscaledDeltaTime; 
                yield return null; 
            }

            backgroundImage.transform.rotation = endRotation;
            startRotation = endRotation;

            yield return new WaitForSecondsRealtime(1f);
            realTimeCountdown -= 1;
        }

        countdownText.text = "GO!";
        StartCoroutine(ZoomImage(backgroundImage));

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1);

        gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
    }

    private IEnumerator ZoomImage(Image image)
    {
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * 10f;
        float zoomDuration = 1f; 
        float elapsedTime = 0f;

        while (elapsedTime < zoomDuration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.unscaledDeltaTime; 
            yield return null;
        }

        rectTransform.localScale = targetScale; 
    }
}
