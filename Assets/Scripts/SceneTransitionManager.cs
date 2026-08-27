using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    public CanvasGroup fadeCanvas;
    public float fadeOutDuration = 0.5f;
    public float fadeInDuration = 0.5f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private IEnumerator Start()
    {
        fadeCanvas.alpha = 1f;
        fadeCanvas.blocksRaycasts = true;

        yield return Fade(0f, fadeInDuration);

        fadeCanvas.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(Transition(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
            StartCoroutine(Transition(sceneIndex));
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;
        fadeCanvas.blocksRaycasts = true;

        yield return Fade(1f, fadeOutDuration);

        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return Fade(0f, fadeInDuration);

        fadeCanvas.blocksRaycasts = false;
        isTransitioning = false;
    }

    private IEnumerator Transition(int sceneIndex)
    {
        isTransitioning = true;
        fadeCanvas.blocksRaycasts = true;

        yield return Fade(1f, fadeOutDuration);

        SceneManager.LoadScene(sceneIndex);

        yield return null;

        yield return Fade(0f, fadeInDuration);

        fadeCanvas.blocksRaycasts = false;
        isTransitioning = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvas.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}