using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite portrait;

    [Header("Dialogue")]
    [TextArea(3, 10)]
    public string[] dialogueLines;

    [Header("Typing")]
    public float typingSpeed = 0.03f;

    [Header("Auto Progress")]
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
    public bool[] endDialogueLines;

    [Header("Voice")]
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex; //dialogue line where choices appear
    public string[] choices; //player response options
    public int[] nextDialogueIndices; //where choice leads
}