using System;
using System.Collections;
using UnityEngine;

public class MenuPanelTransition : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float holdDuration = 0.1f;

    public bool IsBusy { get; private set; }

    public void Switch(GameObject fromPanel, GameObject toPanel)
    {
        if (IsBusy)
            return;

        StartCoroutine(SwitchRoutine(fromPanel, toPanel));
    }

    public void Show(GameObject panel, Action onComplete = null)
    {
        if (IsBusy)
            return;

        StartCoroutine(ShowRoutine(panel, onComplete));
    }

    public void Hide(GameObject panel, Action onComplete = null)
    {
        if (IsBusy)
            return;

        StartCoroutine(HideRoutine(panel, onComplete));
    }

    public void ShowImmediate(GameObject panel, bool show)
    {
        CanvasGroup group = GetGroup(panel);

        panel.SetActive(show);

        group.alpha = show ? 1f : 0f;
        group.interactable = show;
        group.blocksRaycasts = show;
    }

    private IEnumerator SwitchRoutine(GameObject fromPanel, GameObject toPanel)
    {
        IsBusy = true;

        CanvasGroup fromGroup = GetGroup(fromPanel);
        CanvasGroup toGroup = GetGroup(toPanel);

        fromGroup.interactable = false;
        fromGroup.blocksRaycasts = false;

        yield return Fade(fromGroup, 0f);

        fromPanel.SetActive(false);

        if (holdDuration > 0f)
            yield return new WaitForSecondsRealtime(holdDuration);

        toPanel.SetActive(true);

        toGroup.alpha = 0f;
        toGroup.interactable = false;
        toGroup.blocksRaycasts = false;

        yield return Fade(toGroup, 1f);

        toGroup.interactable = true;
        toGroup.blocksRaycasts = true;

        IsBusy = false;
    }

    private IEnumerator ShowRoutine(GameObject panel, Action onComplete)
    {
        IsBusy = true;

        CanvasGroup group = GetGroup(panel);

        panel.SetActive(true);

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        yield return Fade(group, 1f);

        group.interactable = true;
        group.blocksRaycasts = true;

        IsBusy = false;

        onComplete?.Invoke();
    }

    private IEnumerator HideRoutine(GameObject panel, Action onComplete)
    {
        IsBusy = true;

        CanvasGroup group = GetGroup(panel);

        group.interactable = false;
        group.blocksRaycasts = false;

        yield return Fade(group, 0f);

        panel.SetActive(false);

        IsBusy = false;

        onComplete?.Invoke();
    }

    private IEnumerator Fade(CanvasGroup group, float targetAlpha)
    {
        float startAlpha = group.alpha;
        float time = 0f;

        if (fadeDuration <= 0f)
        {
            group.alpha = targetAlpha;
            yield break;
        }

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float progress = time / fadeDuration;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }

        group.alpha = targetAlpha;
    }

    private CanvasGroup GetGroup(GameObject panel)
    {
        CanvasGroup group = panel.GetComponent<CanvasGroup>();

        if (group == null)
            group = panel.AddComponent<CanvasGroup>();

        return group;
    }
}