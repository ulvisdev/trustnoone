using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance { get; private set; }

    [Header("UI")]
    public GameObject cutscenePanel;
    public Image cutsceneImageA;
    public Image cutsceneImageB;
    public UIFade cutsceneFade;
    [SerializeField] private float crossfadeDuration = 0.4f;

    private Image currentImage;
    private Image nextImage;
    private Coroutine crossfadeCoroutine;

    public bool IsPlaying { get; private set; }

    private CutsceneData currentCutscene;
    private int frameIndex;
    private bool pausedByCutscene;
    private Coroutine automaticCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        IsPlaying = false;
        currentCutscene = null;
        automaticCoroutine = null;
        pausedByCutscene = false;

        currentImage = cutsceneImageA;
        nextImage = cutsceneImageB;

        SetImageAlpha(cutsceneImageA, 0f);
        SetImageAlpha(cutsceneImageB, 0f);

        cutsceneImageA.sprite = null;
        cutsceneImageB.sprite = null;

        cutsceneImageA.enabled = false;
        cutsceneImageB.enabled = false;

        if (cutsceneImageA != null)
        {
            cutsceneImageA.sprite = null;
            cutsceneImageA.enabled = false;
        }

        if (cutsceneImageB != null)
        {
            cutsceneImageB.sprite = null;
            cutsceneImageB.enabled = false;
        }

        // if (cutscenePanel != null)
        //     cutscenePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        if (currentCutscene.advanceMode != CutsceneAdvanceMode.Click)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (PauseMenuController.IsPauseMenuOpen)
            return;

        if (UIInputBlocker.IsPointerOverInteractiveUI())
            return;

        NextFrame();
    }

    public bool StartCutscene(CutsceneData cutscene)
    {
        if (cutscene == null)
            return false;

        if (IsPlaying)
            return false;

        if (cutscene.frames == null || cutscene.frames.Length == 0)
            return false;

        if (PauseController.IsGamePaused)
            return false;

        currentCutscene = cutscene;
        frameIndex = 0;
        IsPlaying = true;
        pausedByCutscene = false;

        if (cutsceneFade != null)
            cutsceneFade.Show();
        else
            cutscenePanel.SetActive(true);

        ShowFrame(0);

        if (cutscene.advanceMode == CutsceneAdvanceMode.Narration)
        {
            if (cutscene.narration == null || NarrationController.Instance == null)
            {
                EndCutscene();
                return false;
            }

            if (!NarrationController.Instance.StartNarration(cutscene.narration))
            {
                EndCutscene();
                return false;
            }

            return true;
        }

        if (cutscene.pauseGame)
        {
            PauseController.SetPause(true);
            pausedByCutscene = true;
        }

        if (cutscene.advanceMode == CutsceneAdvanceMode.Automatic)
            StartAutomaticFrame();

        return true;
    }

    public void ShowNarrationFrame(int narrationLineIndex)
    {
        if (!IsPlaying)
            return;

        if (currentCutscene.advanceMode != CutsceneAdvanceMode.Narration)
            return;

        int targetFrame = Mathf.Clamp(narrationLineIndex, 0, currentCutscene.frames.Length - 1);

        ShowFrame(targetFrame);
    }

    private void ShowFrame(int index)
    {
        if (index < 0 || index >= currentCutscene.frames.Length)
            return;

        frameIndex = index;

        Sprite newSprite = currentCutscene.frames[frameIndex].image;

        if (index == 0)
        {
            currentImage.sprite = newSprite;
            currentImage.enabled = newSprite != null;
            SetImageAlpha(currentImage, 1f);

            nextImage.sprite = null;
            nextImage.enabled = false;
            SetImageAlpha(nextImage, 0f);

            return;
        }

        if (crossfadeCoroutine != null)
            StopCoroutine(crossfadeCoroutine);

        crossfadeCoroutine = StartCoroutine(CrossfadeFrame(newSprite));
    }

    private void NextFrame()
    {
        frameIndex++;

        if (frameIndex >= currentCutscene.frames.Length)
        {
            EndCutscene();
            return;
        }

        ShowFrame(frameIndex);
    }

    private IEnumerator CrossfadeFrame(Sprite newSprite)
    {
        nextImage.sprite = newSprite;
        nextImage.enabled = newSprite != null;

        SetImageAlpha(nextImage, 0f);

        float startCurrentAlpha = currentImage.color.a;
        float time = 0f;

        while (time < crossfadeDuration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(time / crossfadeDuration);

            SetImageAlpha(currentImage, Mathf.Lerp(startCurrentAlpha, 0f, t));
            SetImageAlpha(nextImage, Mathf.Lerp(0f, 1f, t));

            yield return null;
        }

        SetImageAlpha(currentImage, 0f);
        SetImageAlpha(nextImage, 1f);

        currentImage.enabled = false;
        currentImage.sprite = null;

        Image oldImage = currentImage;
        currentImage = nextImage;
        nextImage = oldImage;

        crossfadeCoroutine = null;
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void StartAutomaticFrame()
    {
        if (automaticCoroutine != null)
            StopCoroutine(automaticCoroutine);

        automaticCoroutine = StartCoroutine(AutomaticFrame());
    }

    private IEnumerator AutomaticFrame()
    {
        float duration = currentCutscene.frames[frameIndex].duration;

        yield return new WaitForSecondsRealtime(duration);

        automaticCoroutine = null;

        frameIndex++;

        if (frameIndex >= currentCutscene.frames.Length)
        {
            EndCutscene();
            yield break;
        }

        ShowFrame(frameIndex);
        StartAutomaticFrame();
    }

    public void NarrationEnded()
    {
        if (!IsPlaying)
            return;

        if (currentCutscene.advanceMode != CutsceneAdvanceMode.Narration)
            return;

        EndCutscene();
    }

    public void EndCutscene()
    {
        if (!IsPlaying)
            return;

        if (automaticCoroutine != null)
        {
            StopCoroutine(automaticCoroutine);
            automaticCoroutine = null;
        }

        bool keepLastFrame = currentCutscene != null && currentCutscene.keepLastFrameVisible;

        IsPlaying = false;
        currentCutscene = null;

        if (!keepLastFrame)
        {
            if (cutsceneFade != null)
                cutsceneFade.Hide(HideCutscene);
            else
                HideCutscene();
        }

        if (pausedByCutscene)
        {
            PauseController.SetPause(false);
            pausedByCutscene = false;
        }
    }

    public void HideCutscene()
    {
        if (crossfadeCoroutine != null)
        {
            StopCoroutine(crossfadeCoroutine);
            crossfadeCoroutine = null;
        }

        cutsceneImageA.sprite = null;
        cutsceneImageA.enabled = false;
        SetImageAlpha(cutsceneImageA, 0f);

        cutsceneImageB.sprite = null;
        cutsceneImageB.enabled = false;
        SetImageAlpha(cutsceneImageB, 0f);

        cutscenePanel.SetActive(false);
    }
}