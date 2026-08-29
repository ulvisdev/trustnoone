using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonPressFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover")]
    [SerializeField] private float hoverScale = 1.02f;
    [SerializeField] private float hoverOvershootScale = 1.06f;
    [SerializeField] private float hoverPopDuration = 0.08f;
    [SerializeField] private float hoverSettleDuration = 0.06f;

    [Header("Press")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private float releaseOvershootScale = 1.06f;
    [SerializeField] private float pressDuration = 0.08f;
    [SerializeField] private float overshootDuration = 0.08f;
    [SerializeField] private float settleDuration = 0.08f;

    [Header("Unhover")]
    [SerializeField] private float unhoverDuration = 0.08f;

    private Vector3 normalScale;
    private Coroutine scaleCoroutine;
    private bool isHovered;
    private bool isPressed;

    private void Awake()
    {
        normalScale = transform.localScale;
    }

    private void OnDisable()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        isHovered = false;
        isPressed = false;
        transform.localScale = normalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        if (!isPressed)
            StartHoverAnimation();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;

        StartScaleAnimation(normalScale, unhoverDuration);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        StartScaleAnimation(normalScale * pressedScale, pressDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        StartReleaseAnimation();
    }

    private void StartHoverAnimation()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(HoverRoutine());
    }

    private IEnumerator HoverRoutine()
    {
        yield return ScaleRoutine(normalScale * hoverOvershootScale, hoverPopDuration);
        yield return ScaleRoutine(normalScale * hoverScale, hoverSettleDuration);

        scaleCoroutine = null;
    }

    private void StartScaleAnimation(Vector3 targetScale, float duration)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale, duration));
    }

    private void StartReleaseAnimation()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ReleaseRoutine());
    }

    private IEnumerator ReleaseRoutine()
    {
        float targetScale = isHovered ? hoverScale : 1f;
        float overshootScale = isHovered ? releaseOvershootScale : releaseOvershootScale - 0.02f;

        yield return ScaleRoutine(normalScale * overshootScale, overshootDuration);
        yield return ScaleRoutine(normalScale * targetScale, settleDuration);

        scaleCoroutine = null;
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.Lerp(startScale, targetScale, eased);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}