using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public Image backgroundGroup;
    public TextMeshProUGUI titleGroup;
    public Image logoGroup;
    public TextMeshProUGUI loadingTextGroup;

    public float fadeDuration = 1f;
    public float delayBetweenFades = 0.5f;
    public float delayAfterAllVisible = 2f;

    void Start()
    {
        SetAlpha(backgroundGroup, 0);
        SetAlpha(titleGroup, 0);
        SetAlpha(logoGroup, 0);
        SetAlpha(loadingTextGroup, 0);

        StartCoroutine(PlayIntroAndLoadScene());
    }

    IEnumerator PlayIntroAndLoadScene()
    {
        // Fondo
        yield return StartCoroutine(FadeImage(backgroundGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        // Título
        yield return StartCoroutine(FadeText(titleGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        // Logo
        yield return StartCoroutine(FadeImage(logoGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        // Texto "Loading..." SIN fade, aparece de golpe
        SetAlpha(loadingTextGroup, 1f);
        yield return new WaitForSeconds(delayAfterAllVisible);

        // Cargar la escena del menú
        SceneManager.LoadScene("MenuScene");
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void SetAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    IEnumerator FadeImage(Image img, float targetAlpha, float duration)
    {
        float startAlpha = img.color.a;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetAlpha(img, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(img, targetAlpha);
    }

    IEnumerator FadeText(TextMeshProUGUI tmp, float targetAlpha, float duration)
    {
        float startAlpha = tmp.color.a;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetAlpha(tmp, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(tmp, targetAlpha);
    }
}
