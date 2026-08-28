using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrationController : MonoBehaviour
{
    public static NarrationController Instance { get; private set; }

    [Header("Text Effects")]
    public float defaultShakeStrength = 0.5f;
    public float defaultWaveStrength = 0.5f;

    [Header("Text Pop")]
    public float popScale = 1.18f;
    public float popDuration = 0.08f;

    [Header("UI")]
    public GameObject narrationPanel;
    public UIFade narrationFade;
    public TMP_Text narrationText;
    public TMP_Text headingText;
    public GameObject continueIndicator;
    public Image narrationImage;

    [Header("Voice Over")]
    public AudioSource voiceOverSource;

    public bool IsNarrating { get; private set; }

    private NarrationData currentNarration;
    public UITransition lineTransition;
    private int lineIndex;
    private int narrationStartFrame;
    private int visibleCharacterCount;

    private bool isTyping;
    private bool pausedByNarration;

    private Coroutine currentCoroutine;

    private TMP_MeshInfo[] baseMeshInfo;

    private readonly List<float> shakeStrengths = new List<float>();
    private readonly List<float> waveStrengths = new List<float>();
    private readonly List<float> revealTimes = new List<float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        IsNarrating = false;
        pausedByNarration = false;

        if (narrationPanel != null)
            narrationPanel.SetActive(false);

        if (continueIndicator != null)
            continueIndicator.SetActive(false);
    }

    private void Update()
    {
        if (!IsNarrating)
            return;

        if (lineTransition != null && lineTransition.IsTransitioning)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (PauseMenuController.IsPauseMenuOpen)
            return;

        if (UIInputBlocker.IsPointerOverInteractiveUI())
            return;

        if (Time.frameCount == narrationStartFrame)
            return;

        if (isTyping)
            RevealCurrentLine();
        else
            NextLine();
    }

    public bool StartNarration(NarrationData narration)
    {
    Debug.Log("NARRATION TRIGGER: " + gameObject.name + " | Scene: " + gameObject.scene.name + " | Data: " + narration);

    if (narration == null)
    {
        Debug.LogWarning("Narration rejected: narration is NULL");
        return false;
    }

    if (IsNarrating)
    {
        Debug.LogWarning("Narration rejected: another narration is already running");
        return false;
    }

    if (PauseController.IsGamePaused)
    {
        Debug.LogWarning("Narration rejected: game is already paused");
        return false;
    }

    if (narration.lines == null || narration.lines.Length == 0)
    {
        Debug.LogWarning("Narration rejected: no lines");
        return false;
    }

    currentNarration = narration;
    lineIndex = 0;
    IsNarrating = true;
    narrationStartFrame = Time.frameCount;

        if (narrationFade != null)
            narrationFade.Show();
        else
            narrationPanel.SetActive(true);

        if (headingText != null)
        {
            headingText.text = currentNarration.heading;
            headingText.gameObject.SetActive(!string.IsNullOrWhiteSpace(currentNarration.heading));
        }

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        pausedByNarration = false;

        if (currentNarration.pauseGame)
        {
            PauseController.SetPause(true);
            pausedByNarration = true;
        }

        DisplayCurrentLine();

        return true;
    }

    private void DisplayCurrentLine()
    {
        StopCurrentCoroutine();

        if (voiceOverSource != null)
            voiceOverSource.Stop();

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        NarrationLine line = currentNarration.lines[lineIndex];

        //cutscene controller shows a frame
        if (CutsceneController.Instance != null)
            CutsceneController.Instance.ShowNarrationFrame(lineIndex);

        if (narrationImage != null)
        {
            narrationImage.sprite = line.image;
            narrationImage.gameObject.SetActive(line.image != null);
        }

        string processedText = ProcessCustomTags(line.text);

        narrationText.text = processedText;
        narrationText.ForceMeshUpdate();

        baseMeshInfo = narrationText.textInfo.CopyMeshInfoVertexData();

        visibleCharacterCount = 0;
        revealTimes.Clear();

        for (int i = 0; i < narrationText.textInfo.characterCount; i++)
            revealTimes.Add(-1f);

        if (voiceOverSource != null && line.voiceOver != null)
            voiceOverSource.PlayOneShot(line.voiceOver);

        currentCoroutine = StartCoroutine(TypeCurrentLine());
    }

    private IEnumerator TypeCurrentLine()
    {
        isTyping = true;

        int totalCharacters = narrationText.textInfo.characterCount;

        while (visibleCharacterCount < totalCharacters)
        {
            visibleCharacterCount++;
            revealTimes[visibleCharacterCount - 1] = Time.unscaledTime;

            char currentCharacter = narrationText.textInfo.characterInfo[visibleCharacterCount - 1].character;
            float delay = currentNarration.typingSpeed;

            if (currentCharacter == '.' || currentCharacter == '!' || currentCharacter == '?')
                delay += currentNarration.punctuationPause;

            yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;
        currentCoroutine = null;

        CurrentLineFinished();
    }

    private void RevealCurrentLine()
    {
        StopCurrentCoroutine();

        visibleCharacterCount = narrationText.textInfo.characterCount;

        for (int i = 0; i < revealTimes.Count; i++)
            revealTimes[i] = Time.unscaledTime - popDuration;

        isTyping = false;

        CurrentLineFinished();
    }

    private void CurrentLineFinished()
    {
        NarrationLine line = currentNarration.lines[lineIndex];

        if (line.autoProgress)
        {
            if (continueIndicator != null)
                continueIndicator.SetActive(false);

            currentCoroutine = StartCoroutine(AutoProgress(line.autoProgressDelay));
        }
        else
        {
            if (continueIndicator != null)
                continueIndicator.SetActive(true);
        }
    }

    private IEnumerator AutoProgress(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        currentCoroutine = null;

        NextLine();
    }

    private void NextLine()
    {
        StopCurrentCoroutine();

        int nextIndex = lineIndex + 1;

        if (nextIndex >= currentNarration.lines.Length)
        {
            EndNarration();
            return;
        }

        if (lineTransition != null)
        {
            lineTransition.Swap(() =>
            {
                lineIndex = nextIndex;
                DisplayCurrentLine();
            });
        }
        else
        {
            lineIndex = nextIndex;
            DisplayCurrentLine();
        }
    }

    public void EndNarration()
    {
        StopCurrentCoroutine();

        if (voiceOverSource != null)
            voiceOverSource.Stop();

        IsNarrating = false;
        isTyping = false;

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        if (narrationFade != null)
            narrationFade.Hide(FinishNarrationClose);
        else
        {
            narrationPanel.SetActive(false);
            FinishNarrationClose();
        }
    }

    private void FinishNarrationClose()
    {
        narrationText.text = "";

        if (headingText != null)
            headingText.text = "";

        if (narrationImage != null)
        {
            narrationImage.sprite = null;
            narrationImage.gameObject.SetActive(false);
        }

        currentNarration = null;

        if (pausedByNarration)
        {
            PauseController.SetPause(false);
            pausedByNarration = false;
        }

        //end cutscene together with narration
        if (CutsceneController.Instance != null)
            CutsceneController.Instance.NarrationEnded();
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine == null)
            return;

        StopCoroutine(currentCoroutine);
        currentCoroutine = null;
    }

    private string ProcessCustomTags(string source)
    {
        shakeStrengths.Clear();
        waveStrengths.Clear();

        if (string.IsNullOrEmpty(source))
            return "";

        StringBuilder result = new StringBuilder();

        float currentShake = 0f;
        float currentWave = 0f;

        int i = 0;

        while (i < source.Length)
        {
            if (source[i] == '[')
            {
                int closingBracket = source.IndexOf(']', i);

                if (closingBracket >= 0)
                {
                    string tag = source.Substring(i + 1, closingBracket - i - 1);
                    bool handled = true;

                    if (tag == "shake")
                        currentShake = defaultShakeStrength;
                    else if (tag.StartsWith("shake="))
                        currentShake = ParseStrength(tag.Substring("shake=".Length));
                    else if (tag == "/shake")
                        currentShake = 0f;
                    else if (tag == "wave")
                        currentWave = defaultWaveStrength;
                    else if (tag.StartsWith("wave="))
                        currentWave = ParseStrength(tag.Substring("wave=".Length));
                    else if (tag == "/wave")
                        currentWave = 0f;
                    else if (tag == "item")
                        result.Append("<color=#FFD54A>");
                    else if (tag == "/item")
                        result.Append("</color>");
                    else if (tag == "danger")
                        result.Append("<color=#FF5555>");
                    else if (tag == "/danger")
                        result.Append("</color>");
                    else
                        handled = false;

                    if (handled)
                    {
                        i = closingBracket + 1;
                        continue;
                    }
                }
            }

            if (source[i] == '<')
            {
                int richTextEnd = source.IndexOf('>', i);

                if (richTextEnd >= 0)
                {
                    result.Append(source.Substring(i, richTextEnd - i + 1));
                    i = richTextEnd + 1;
                    continue;
                }
            }

            result.Append(source[i]);

            shakeStrengths.Add(currentShake);
            waveStrengths.Add(currentWave);

            i++;
        }

        return result.ToString();
    }

    private float ParseStrength(string value)
    {
        float strength;

        if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out strength))
            return 1f;

        return Mathf.Clamp(strength, 0f, 10f);
    }

    private void LateUpdate()
    {
        if (!IsNarrating)
            return;

        if (baseMeshInfo == null)
            return;

        TMP_TextInfo textInfo = narrationText.textInfo;

        for (int meshIndex = 0; meshIndex < textInfo.meshInfo.Length; meshIndex++)
        {
            Array.Copy(baseMeshInfo[meshIndex].vertices, textInfo.meshInfo[meshIndex].vertices, baseMeshInfo[meshIndex].vertices.Length);
            Array.Copy(baseMeshInfo[meshIndex].colors32, textInfo.meshInfo[meshIndex].colors32, baseMeshInfo[meshIndex].colors32.Length);
        }

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];

            if (!characterInfo.isVisible)
                continue;

            int materialIndex = characterInfo.materialReferenceIndex;
            int vertexIndex = characterInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            if (i >= visibleCharacterCount)
            {
                for (int j = 0; j < 4; j++)
                {
                    Color32 color = colors[vertexIndex + j];
                    color.a = 0;
                    colors[vertexIndex + j] = color;
                }

                continue;
            }

            Vector3 offset = Vector3.zero;

            float shakeStrength = GetShakeStrength(i);
            float waveStrength = GetWaveStrength(i);

            if (shakeStrength > 0f)
            {
                float shakeX = Mathf.Sin(Time.unscaledTime * 35f + i * 13.17f);
                float shakeY = Mathf.Cos(Time.unscaledTime * 41f + i * 9.41f);

                offset.x += shakeX * 1.5f * shakeStrength;
                offset.y += shakeY * 1.5f * shakeStrength;
            }

            if (waveStrength > 0f)
            {
                float wave = Mathf.Sin(Time.unscaledTime * 8f + i * 0.6f);
                offset.y += wave * 2f * waveStrength;
            }

            for (int j = 0; j < 4; j++)
                vertices[vertexIndex + j] += offset;

            if (i < revealTimes.Count && revealTimes[i] >= 0f)
            {
                float progress = Mathf.Clamp01((Time.unscaledTime - revealTimes[i]) / popDuration);
                float scale = Mathf.Lerp(popScale, 1f, Mathf.SmoothStep(0f, 1f, progress));
                Vector3 center = (vertices[vertexIndex] + vertices[vertexIndex + 2]) * 0.5f;

                for (int j = 0; j < 4; j++)
                    vertices[vertexIndex + j] = center + (vertices[vertexIndex + j] - center) * scale;
            }
        }

        for (int meshIndex = 0; meshIndex < textInfo.meshInfo.Length; meshIndex++)
        {
            textInfo.meshInfo[meshIndex].mesh.vertices = textInfo.meshInfo[meshIndex].vertices;
            textInfo.meshInfo[meshIndex].mesh.colors32 = textInfo.meshInfo[meshIndex].colors32;
            narrationText.UpdateGeometry(textInfo.meshInfo[meshIndex].mesh, meshIndex);
        }
    }

    private float GetShakeStrength(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= shakeStrengths.Count)
            return 0f;

        return shakeStrengths[characterIndex];
    }

    private float GetWaveStrength(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= waveStrengths.Count)
            return 0f;

        return waveStrengths[characterIndex];
    }
}