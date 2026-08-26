using UnityEngine;

[CreateAssetMenu(fileName = "NewNarration", menuName = "Story/Narration Data")]
public class NarrationData : ScriptableObject
{
    [Header("Optional Heading")]
    public string heading;

    [Header("Typing")]
    public float typingSpeed = 0.03f;
    public float punctuationPause = 0.15f;

    [Header("Behaviour")]
    public bool pauseGame = true;

    [Header("Narration Lines")]
    public NarrationLine[] lines;
}

[System.Serializable]
public class NarrationLine
{
    [TextArea(3, 10)]
    public string text;

    public Sprite image;

    public bool autoProgress;
    public float autoProgressDelay = 1.5f;

    [Header("Voice Over")]
    public AudioClip voiceOver;
}