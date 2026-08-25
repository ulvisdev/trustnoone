using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DialoguePortraitAnimator : MonoBehaviour
{
    [Header("Blink")]
    public float minimumBlinkDelay = 2f;
    public float maximumBlinkDelay = 5f;
    public float blinkDuration = 0.12f;

    [Header("Talking")]
    public float mouthInterval = 0.09f;

    private Image portraitImage;

    private Sprite idleSprite;
    private Sprite blinkSprite;
    private Sprite talkingSprite;

    private bool isTalking;
    private bool mouthOpen;
    private bool isBlinking;

    private float mouthTimer;
    private float blinkTimer;
    private float blinkTimeRemaining;

    private void Awake()
    {
        portraitImage = GetComponent<Image>();
    }

    public void Configure(Sprite idle, Sprite blink, Sprite talking)
    {
        idleSprite = idle;
        blinkSprite = blink;
        talkingSprite = talking;

        isTalking = false;
        mouthOpen = false;
        isBlinking = false;

        portraitImage.sprite = idleSprite;

        ResetBlinkTimer();
    }

    public void SetTalking(bool talking)
    {
        isTalking = talking;
        mouthOpen = false;
        isBlinking = false;

        if (portraitImage != null)
            portraitImage.sprite = idleSprite;

        if (talking)
            mouthTimer = 0f;
        else
            ResetBlinkTimer();
    }

    private void Update()
    {
        if (portraitImage == null || idleSprite == null)
            return;

        if (isTalking)
            UpdateTalking();
        else
            UpdateBlinking();
    }

    private void UpdateTalking()
    {
        mouthTimer -= Time.unscaledDeltaTime;

        if (mouthTimer > 0f)
            return;

        mouthOpen = !mouthOpen;

        if (mouthOpen && talkingSprite != null)
            portraitImage.sprite = talkingSprite;
        else
            portraitImage.sprite = idleSprite;

        mouthTimer = mouthInterval;
    }

    private void UpdateBlinking()
    {
        if (isBlinking)
        {
            blinkTimeRemaining -= Time.unscaledDeltaTime;

            if (blinkTimeRemaining <= 0f)
            {
                isBlinking = false;
                portraitImage.sprite = idleSprite;
                ResetBlinkTimer();
            }

            return;
        }

        blinkTimer -= Time.unscaledDeltaTime;

        if (blinkTimer > 0f)
            return;

        if (blinkSprite == null)
        {
            ResetBlinkTimer();
            return;
        }

        isBlinking = true;
        portraitImage.sprite = blinkSprite;
        blinkTimeRemaining = blinkDuration;
    }

    private void ResetBlinkTimer()
    {
        blinkTimer = Random.Range(minimumBlinkDelay, maximumBlinkDelay);
    }
}