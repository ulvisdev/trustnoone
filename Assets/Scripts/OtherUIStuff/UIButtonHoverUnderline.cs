using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Underline")]
    [SerializeField] private RectTransform underline;
    [SerializeField] private float startScale = 0.8f;
    [SerializeField] private float overshootScale = 1.1f;
    [SerializeField] private float popDuration = 0.08f;
    [SerializeField] private float settleDuration = 0.06f;
    [SerializeField] private float hideDuration = 0.08f;

    [Header("Optional Text Pop")]
    [SerializeField] private RectTransform hoverText;
    [SerializeField] private float textOvershootScale = 1.05f;

    private Coroutine animationCoroutine;
    private Vector3 textNormalScale;

    private void Awake()
    {
        underline.localScale = new Vector3(startScale, 1f, 1f);
        underline.gameObject.SetActive(false);

        if (hoverText != null)
            textNormalScale = hoverText.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        underline.gameObject.SetActive(true);
        animationCoroutine = StartCoroutine(ShowRoutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        underline.localScale = new Vector3(startScale, 1f, 1f);

        if (hoverText != null)
            hoverText.localScale = textNormalScale;

        Coroutine underlinePop = StartCoroutine(ScaleUnderlineX(overshootScale, popDuration));
        Coroutine textPop = hoverText != null ? StartCoroutine(ScaleText(textNormalScale * textOvershootScale, popDuration)) : null;

        yield return underlinePop;

        Coroutine underlineSettle = StartCoroutine(ScaleUnderlineX(1f, settleDuration));
        Coroutine textSettle = hoverText != null ? StartCoroutine(ScaleText(textNormalScale, settleDuration)) : null;

        yield return underlineSettle;

        animationCoroutine = null;
    }

    private IEnumerator HideRoutine()
    {
        Coroutine underlineHide = StartCoroutine(ScaleUnderlineX(startScale, hideDuration));
        Coroutine textSettle = hoverText != null ? StartCoroutine(ScaleText(textNormalScale, hideDuration)) : null;

        yield return underlineHide;

        underline.gameObject.SetActive(false);
        animationCoroutine = null;
    }

    private IEnumerator ScaleUnderlineX(float targetScale, float duration)
    {
        float start = underline.localScale.x;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float scale = Mathf.Lerp(start, targetScale, eased);
            underline.localScale = new Vector3(scale, 1f, 1f);

            yield return null;
        }

        underline.localScale = new Vector3(targetScale, 1f, 1f);
    }

    private IEnumerator ScaleText(Vector3 targetScale, float duration)
    {
        Vector3 start = hoverText.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            hoverText.localScale = Vector3.Lerp(start, targetScale, eased);

            yield return null;
        }

        hoverText.localScale = targetScale;
    }
}