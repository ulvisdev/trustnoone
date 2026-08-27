using UnityEngine;

public enum CutsceneAdvanceMode
{
    Narration,
    Click,
    Automatic
}

[CreateAssetMenu(fileName = "NewCutscene", menuName = "Cutscene")]
public class CutsceneData : ScriptableObject
{
    [Header("Frames")]
    public CutsceneFrame[] frames;

    [Header("Progression")]
    public CutsceneAdvanceMode advanceMode = CutsceneAdvanceMode.Narration;

    [Header("Narration")]
    public NarrationData narration;

    [Header("Ending")]
    public bool keepLastFrameVisible;

    [Header("Standalone Cutscene")]
    public bool pauseGame = true;
}

[System.Serializable]
public class CutsceneFrame
{
    public Sprite image;
    public float duration = 2f;
}