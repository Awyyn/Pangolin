using UnityEngine;
using TMPro;

public class SkipCutscene : MonoBehaviour
{
    public TMP_Text skipText;
    public float fadeDelay = 1f;
    public float fadeDuration = 1f;

    private void OnEnable()
    {
        skipText.alpha = 1f;
        Invoke(nameof(FadeOut), fadeDelay);
    }

    void FadeOut()
    {
        StartCoroutine(FadeCoroutine());
    }

    System.Collections.IEnumerator FadeCoroutine()
    {
        float elapsed = 0f;
        float startAlpha = skipText.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            skipText.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        skipText.alpha = 0f;
    }
}