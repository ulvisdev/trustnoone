using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite portrait;
    public Sprite blinkPortrait;
    public Sprite talkingPortrait;

    [Header("NPC Voice")]
    public AudioClip voiceSound;
    public float voicePitch = 1f;

    [Header("Typing")]
    public float typingSpeed = 0.05f;
    public float periodPause = 0.2f;

    [Header("Conversation Memory")]
    public string firstConversationCompleteFlag;
    public int repeatConversationStartNode = -1;

    [Header("Optional Narration after Dialogue end")]
    public NarrationData narrationAfterDialogue;

    [Header("Dialogue")]
    public DialogueNode[] nodes;
}

public enum DialogueSpeaker
{
    NPC,
    Player
}

[System.Serializable]
public class DialogueNode
{
    [Header("Speaker")]
    public DialogueSpeaker speaker;

    [TextArea(3, 10)]
    public string text;

    [Header("Navigation")]
    [Tooltip("-1 means continue to the next node")]
    public int nextNodeIndex = -1;

    public bool endDialogue;

    [Header("Automatic Progress")]
    public bool autoProgress;
    public float autoProgressDelay = 1.5f;

    [Header("Conditions")]
    [Tooltip("This node only appears if this flag exists")]
    public string requiredFlag;

    [Tooltip("This node is skipped if this flag exists")]
    public string forbiddenFlag;

    [Header("Effect")]
    [Tooltip("This flag is unlocked when this node is reached")]
    public string flagToSet;

    [Header("Choices")]
    public DialogueChoice[] choices;

    [Header("Player Name")]
    [Tooltip("Shows the player name input after this line finishes")]
    public bool askForPlayerName;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea(2, 5)]
    public string choiceText;

    [Tooltip("Node this choice leads to, -1 means continue normally")]
    public int nextNodeIndex = -1;

    [Header("Conditions")]
    [Tooltip("Choice only appears if this flag exists")]
    public string requiredFlag;

    [Tooltip("Choice is hidden if this flag exists")]
    public string forbiddenFlag;

    [Header("Effect")]
    [Tooltip("Flag unlocked when the player chooses this option")]
    public string flagToSet;

    [Header("Popup")]
    [TextArea(2, 4)]
    public string popupText;
}