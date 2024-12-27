using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Tooltip("The UI Image used for fading (should cover the entire screen)")]
    public Image fadeOverlay; // Assign this in the Inspector

    private void Awake()
    {
        if (fadeOverlay == null)
        {
            Debug.LogError("FadeOverlay Image not assigned! Please assign it in the Inspector.");
        }
    }

    /// <summary>
    /// Fades the screen to black (fade in effect).
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds.</param>
    public void FadeIn(float duration)
    {
        StartCoroutine(Fade(0f, 1f, duration));
    }

    /// <summary>
    /// Fades the screen back to transparent (fade out effect).
    /// </summary>
    /// <param name="duration">Duration of the fade in seconds.</param>
    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(1f, 0f, duration));
    }

    public IEnumerator FadeInIEnumerator(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha); // Ajusta el Alpha
            yield return null; // Espera al siguiente frame
        }

        fadeOverlay.color = new Color(0, 0, 0, 1f); // Asegura el estado final
    }

    public IEnumerator FadeOutIEnumerator(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha); // Ajusta el Alpha
            yield return null; // Espera al siguiente frame
        }

        fadeOverlay.color = new Color(0, 0, 0, 0f); // Asegura el estado final
    }


    /// <summary>
    /// Handles the fade effect by interpolating the alpha value of the fadeOverlay.
    /// </summary>
    /// <param name="startAlpha">Starting alpha value.</param>
    /// <param name="endAlpha">Ending alpha value.</param>
    /// <param name="duration">Duration of the fade effect.</param>
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeOverlay.color = new Color(0, 0, 0, alpha); // Adjust the Alpha only
            yield return null;
        }

        // Ensure the final alpha value is set
        fadeOverlay.color = new Color(0, 0, 0, endAlpha);
    }
}
