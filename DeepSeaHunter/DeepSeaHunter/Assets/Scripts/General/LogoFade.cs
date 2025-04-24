using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoFadeController : MonoBehaviour
{
    public RawImage logoImage;
    public float fadeDuration = 2f;
    public float displayDuration = 2f;

    private void Start()
    {
        StartCoroutine(FadeLogoSequence());
    }

    IEnumerator FadeLogoSequence()
    {
        yield return Fade(0, 1); // Fade in
        yield return new WaitForSeconds(displayDuration);
        yield return Fade(1, 0); // Fade out
        SceneManager.LoadScene("LoadingScreen");
    }

    IEnumerator Fade(float from, float to)
    {
        float time = 0f;
        Color color = logoImage.color;

        while (time < fadeDuration)
        {
            color.a = Mathf.Lerp(from, to, time / fadeDuration);
            logoImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = to;
        logoImage.color = color;
    }
}
