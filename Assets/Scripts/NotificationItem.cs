using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationItem : MonoBehaviour
{
    [Header("References")]
    public TMP_Text notificationText;
    public CanvasGroup canvasGroup;
    public RectTransform visualRect;

    private float displayTime;
    private float fadeTime;
    private float slideDistance;

    public void Show(string message, float newDisplayTime, float newFadeTime, float newSlideDistance)
    {
        notificationText.text = message;
        displayTime = newDisplayTime;
        fadeTime = newFadeTime;
        slideDistance = newSlideDistance;

        StartCoroutine(NotificationRoutine());
    }

    private IEnumerator NotificationRoutine()
    {
        canvasGroup.alpha = 0f;
        visualRect.anchoredPosition = Vector2.left * slideDistance;

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / fadeTime);

            canvasGroup.alpha = progress;
            visualRect.anchoredPosition = Vector2.Lerp(Vector2.left * slideDistance, Vector2.zero, progress);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        visualRect.anchoredPosition = Vector2.zero;

        yield return new WaitForSecondsRealtime(displayTime);

        timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / fadeTime);

            canvasGroup.alpha = 1f - progress;
            visualRect.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.left * slideDistance, progress);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        Destroy(gameObject);
    }
}