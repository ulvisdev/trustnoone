using System;
using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    public float fadeDuration = 0.2f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        fadeCoroutine = StartCoroutine(FadeTo(1f, null));
    }

    public void ShowImmediate()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(true);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide(Action onComplete = null)
    {
        if (!gameObject.activeSelf)
        {
            onComplete?.Invoke();
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTo(0f, () => { gameObject.SetActive(false); onComplete?.Invoke(); }));
    }

    public void HideImmediate()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }

    private IEnumerator FadeTo(float targetAlpha, Action onComplete)
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        if (targetAlpha > 0f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        fadeCoroutine = null;
        onComplete?.Invoke();
    }
}