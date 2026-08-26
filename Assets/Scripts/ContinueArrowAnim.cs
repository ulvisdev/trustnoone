using UnityEngine;

public class ContinueArrowAnim : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 3f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.unscaledTime * moveSpeed) * moveDistance;
        rectTransform.anchoredPosition = startPosition + new Vector2(0f, yOffset);
    }

    private void OnDisable()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = startPosition;
    }
}