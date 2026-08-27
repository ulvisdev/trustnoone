using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool fadeInOnStart = true;

    public bool IsBusy { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        fadeGroup.interactable = false;

        if (fadeInOnStart)
        {
            fadeGroup.alpha = 1f;
            fadeGroup.blocksRaycasts = true;
        }
        else
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator Start()
    {
        if (!fadeInOnStart)
            yield break;

        IsBusy = true;

        yield return null;
        yield return Fade(0f);

        fadeGroup.blocksRaycasts = false;

        IsBusy = false;
    }

    public void LoadScene(string sceneName)
    {
        if (IsBusy)
            return;

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        IsBusy = true;
        fadeGroup.blocksRaycasts = true;

        yield return Fade(1f);

        PauseController.SetPause(false);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
            yield return null;

        yield return null;
        yield return Fade(0f);

        fadeGroup.blocksRaycasts = false;
        IsBusy = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeGroup.alpha;

        if (fadeDuration <= 0f)
        {
            fadeGroup.alpha = targetAlpha;
            yield break;
        }

        float startTime = Time.realtimeSinceStartup;

        while (fadeGroup.alpha != targetAlpha)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            fadeGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            if (progress >= 1f)
                break;

            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
    }
}