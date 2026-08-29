using System.Collections;
using UnityEngine;

public class UIMenuEntrance : MonoBehaviour
{
    [Header("Elements in appearance order")]
    [SerializeField] private RectTransform[] elements;

    [Header("Animation")]
    [SerializeField] private float slideDistance = 30f;
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float delayBetweenElements = 0.08f;

    [Header("Pop")]
    [SerializeField] private float startScale = 0.9f;
    [SerializeField] private float overshootScale = 1.05f;

    private Vector2[] originalPositions;
    private Vector3[] originalScales;

    private Coroutine animationCoroutine;

    [SerializeField] private bool playOnEnable = true;

    private void Awake()
    {
        originalPositions = new Vector2[elements.Length];
        originalScales = new Vector3[elements.Length];

        for (int i = 0; i < elements.Length; i++)
        {
            originalPositions[i] = elements[i].anchoredPosition;
            originalScales[i] = elements[i].localScale;

            if (elements[i].GetComponent<CanvasGroup>() == null)
                elements[i].gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Play()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlaySequence());
    }

    private void OnEnable()
    {
        if (playOnEnable)
            StartCoroutine(PlayNextFrame());
    }

    private IEnumerator PlayNextFrame()
    {
        yield return null;
        Play();
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    private IEnumerator PlaySequence()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            RectTransform element = elements[i];
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();

            element.anchoredPosition = originalPositions[i] + Vector2.down * slideDistance;
            element.localScale = originalScales[i] * startScale;
            canvasGroup.alpha = 0f;
        }

        for (int i = 0; i < elements.Length; i++)
        {
            StartCoroutine(AnimateElement(i));
            yield return new WaitForSecondsRealtime(delayBetweenElements);
        }
    }

    private IEnumerator AnimateElement(int index)
    {
        RectTransform element = elements[index];
        CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();

        Vector2 startPosition = originalPositions[index] + Vector2.down * slideDistance;
        Vector2 endPosition = originalPositions[index];

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            element.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);

            canvasGroup.alpha = eased;
            float scale;

            if (t < 0.7f)
            {
                float scaleT = t / 0.7f;
                scale = Mathf.Lerp(startScale, overshootScale, scaleT);
            }
            else
            {
                float scaleT = (t - 0.7f) / 0.3f;
                scale = Mathf.Lerp(overshootScale, 1f, scaleT);
            }

            element.localScale = originalScales[index] * scale;
            yield return null;
        }

        element.anchoredPosition = endPosition;
        element.localScale = originalScales[index];
        canvasGroup.alpha = 1f;
    }
}