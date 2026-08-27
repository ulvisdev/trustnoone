using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance { get; private set; }

    [Header("UI")]
    public GameObject cutscenePanel;
    public Image cutsceneImage;
    public UIFade cutsceneFade;
    public UITransition frameTransition;

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

        if (cutsceneImage != null)
        {
            cutsceneImage.sprite = null;
            cutsceneImage.enabled = false;
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

        if (Input.GetMouseButtonDown(0))
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

        if (frameTransition != null)
        {
            frameTransition.Swap(() =>
            {
                cutsceneImage.sprite = newSprite;
                cutsceneImage.enabled = newSprite != null;
            });
        }
        else
        {
            cutsceneImage.sprite = newSprite;
            cutsceneImage.enabled = newSprite != null;
        }
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
        cutsceneImage.sprite = null;
        cutsceneImage.enabled = false;
        cutscenePanel.SetActive(false);
    }
}