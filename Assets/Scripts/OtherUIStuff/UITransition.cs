using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UITransition : MonoBehaviour
{
    public float fadeOutDuration = 0.08f;
    public float fadeInDuration = 0.12f;

    public bool IsTransitioning { get; private set; }

    private CanvasGroup canvasGroup;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        EnsureInitialized();
        canvasGroup.alpha = 1f;
    }

    public void Swap(Action swapAction, Action onComplete = null)
    {
        StopTransition();
        transitionCoroutine = StartCoroutine(SwapRoutine(swapAction, onComplete));
    }

    public void FadeIn(Action onComplete = null)
    {
        EnsureInitialized();
        StopTransition();
        transitionCoroutine = StartCoroutine(FadeInRoutine(onComplete));
    }

    public void FadeOut(Action onComplete = null)
    {
        EnsureInitialized();
        StopTransition();
        transitionCoroutine = StartCoroutine(FadeOutRoutine(onComplete));
    }

    public void ShowImmediate()
    {
        EnsureInitialized();
        StopTransition();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator SwapRoutine(Action swapAction, Action onComplete)
    {
        EnsureInitialized();

        IsTransitioning = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return FadeTo(0f, fadeOutDuration);

        swapAction?.Invoke();

        yield return FadeTo(1f, fadeInDuration);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        IsTransitioning = false;
        transitionCoroutine = null;

        onComplete?.Invoke();
    }

    private IEnumerator FadeInRoutine(Action onComplete)
    {
        IsTransitioning = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.alpha = 0f;

        yield return FadeTo(1f, fadeInDuration);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        IsTransitioning = false;
        transitionCoroutine = null;

        onComplete?.Invoke();
    }

    private IEnumerator FadeOutRoutine(Action onComplete)
    {
        IsTransitioning = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return FadeTo(0f, fadeOutDuration);

        IsTransitioning = false;
        transitionCoroutine = null;

        onComplete?.Invoke();
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = canvasGroup.alpha;

        if (duration <= 0f)
        {
            canvasGroup.alpha = target;
            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }

    private void StopTransition()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        IsTransitioning = false;
    }

    public void HideImmediate()
    {
        EnsureInitialized();
        StopTransition();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void EnsureInitialized()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
}