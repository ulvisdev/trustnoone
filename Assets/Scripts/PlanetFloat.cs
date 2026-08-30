using UnityEngine;

public class PlanetFloat : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float moveAmount = 8f;
    [SerializeField] private float moveSpeed = 0.3f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Offset")]
    [SerializeField] private float startOffset = 0f;

    [Header("Bop")]
    [SerializeField] private float bopAmount = 2f;
    [SerializeField] private float bopSpeed = 1.2f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float floatOffset = Mathf.Sin((Time.unscaledTime + startOffset) * moveSpeed) * moveAmount;
        float bopOffset = Mathf.Sin((Time.unscaledTime + startOffset) * bopSpeed) * bopAmount;
        float yOffset = floatOffset + bopOffset;

        rectTransform.anchoredPosition =
            startPosition + new Vector2(0f, yOffset);

        rectTransform.Rotate(0f, 0f, rotationSpeed * Time.unscaledDeltaTime);
    }
}