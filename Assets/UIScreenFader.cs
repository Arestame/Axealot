using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Tooltip("The UI Image used for fading (should cover the entire screen)")]
    public Image fadeOverlay;

    private bool isFading = false; // Prevent multiple fade calls

    private void Awake()
    {
        if (fadeOverlay == null)
        {
            Debug.LogError("FadeOverlay Image not assigned! Please assign it in the Inspector.");
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        if (isFading) yield break; // Prevent multiple calls
        isFading = true;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha); // Adjust the Alpha
            yield return null;
        }

        fadeOverlay.color = new Color(0, 0, 0, 1f);
        isFading = false;
    }

    public IEnumerator FadeOut(float duration)
    {
        if (isFading) yield break; // Prevent multiple calls
        isFading = true;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha); // Adjust the Alpha
            yield return null;
        }

        fadeOverlay.color = new Color(0, 0, 0, 0f);
        isFading = false;
    }
}

