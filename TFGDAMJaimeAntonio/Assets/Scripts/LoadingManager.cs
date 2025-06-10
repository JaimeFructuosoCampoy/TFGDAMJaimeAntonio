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

    /// <summary>
    /// Ejecuta la secuencia completa de intro animada y carga la escena del menú.
    /// Muestra los elementos UI en orden: fondo, título, logo y texto de carga.
    /// </summary>
    /// <returns>Corrutina que gestiona toda la secuencia temporal</returns>
    IEnumerator PlayIntroAndLoadScene()
    {
        //Fondo
        yield return StartCoroutine(FadeImage(backgroundGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        //Título
        yield return StartCoroutine(FadeText(titleGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        //Logo
        yield return StartCoroutine(FadeImage(logoGroup, 1f, fadeDuration));
        yield return new WaitForSeconds(delayBetweenFades);

        //Texto "Loading..." SIN fade, aparece de golpe
        SetAlpha(loadingTextGroup, 1f);
        yield return new WaitForSeconds(delayAfterAllVisible);

        // Cargar la escena del menú
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Establece la transparencia (canal alpha) de una imagen UI.
    /// </summary>
    /// <param name="img">Componente Image al que modificar la transparencia</param>
    /// <param name="alpha">Valor de transparencia entre 0 (invisible) y 1 (opaco)</param>
    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    /// <summary>
    /// Establece la transparencia (canal alpha) de un texto TextMeshPro.
    /// </summary>
    /// <param name="tmp">Componente TextMeshProUGUI al que modificar la transparencia</param>
    /// <param name="alpha">Valor de transparencia entre 0 (invisible) y 1 (opaco)</param>
    void SetAlpha(TextMeshProUGUI tmp, float alpha)
    {
        Color c = tmp.color;
        c.a = alpha;
        tmp.color = c;
    }

    /// <summary>
    /// Realiza una transición suave de transparencia en una imagen durante un tiempo determinado.
    /// Utiliza interpolación lineal para crear un efecto de fade.
    /// </summary>
    /// <param name="img">Imagen a la que aplicar el fade</param>
    /// <param name="targetAlpha">Transparencia objetivo (0-1)</param>
    /// <param name="duration">Duración de la transición en segundos</param>
    /// <returns>Corrutina que gestiona la animación frame a frame</returns>
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

    /// <summary>
    /// Realiza una transición suave de transparencia en un texto TextMeshPro durante un tiempo determinado.
    /// Utiliza interpolación lineal para crear un efecto de fade.
    /// </summary>
    /// <param name="tmp">Texto al que aplicar el fade</param>
    /// <param name="targetAlpha">Transparencia objetivo (0-1)</param>
    /// <param name="duration">Duración de la transición en segundos</param>
    /// <returns>Corrutina que gestiona la animación frame a frame</returns>
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
