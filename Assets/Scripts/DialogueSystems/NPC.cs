using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCDialogue dialogueData;

    private DialogueController dialogueUI;

    private int dialogueIndex;

    private bool isTyping;
    private bool isDialogueActive;
    private bool isWaitingForChoice;
    private bool isWaitingForName;

    private int dialogueStartFrame;

    private string currentFullLine;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (Time.frameCount == dialogueStartFrame)
            return;

        if (isWaitingForChoice)
            return;

        if (isWaitingForName)
            return;

        NextLine();
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null)
            return;

        if (PauseController.IsGamePaused && !isDialogueActive)
            return;

        if (!isDialogueActive)
            StartDialogue();
        else
            NextLine();
    }

    public void StartDialogue()
    {
        if (dialogueData.nodes == null || dialogueData.nodes.Length == 0)
            return;

        isDialogueActive = true;
        dialogueStartFrame = Time.frameCount;

        dialogueIndex = 0;

        if (DialogueState.Instance != null && !string.IsNullOrWhiteSpace(dialogueData.firstConversationCompleteFlag) &&
            DialogueState.Instance.HasFlag(dialogueData.firstConversationCompleteFlag) && dialogueData.repeatConversationStartNode >= 0)
        {
            dialogueIndex = dialogueData.repeatConversationStartNode;
        }

        dialogueUI.ShowDialogue(true);
        PauseController.SetPause(true);

        DisplayCurrentNode();
    }

    private void DisplayCurrentNode()
    {
        StopAllCoroutines();

        dialogueUI.ShowContinueArrow(false);
        dialogueUI.ClearChoices();

        isWaitingForChoice = false;
        isWaitingForName = false;

        dialogueIndex = FindNextValidNode(dialogueIndex);

        if (dialogueIndex == -1)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = dialogueData.nodes[dialogueIndex];

        if (!string.IsNullOrWhiteSpace(node.flagToSet) && DialogueState.Instance != null)
            DialogueState.Instance.SetFlag(node.flagToSet);

        dialogueUI.SetSpeaker(node.speaker, dialogueData);
        currentFullLine = FormatText(node.text);

        StartCoroutine(TypeLine(node));
    }

    private IEnumerator TypeLine(DialogueNode node)
    {
        isTyping = true;

        dialogueUI.ShowContinueArrow(false);
        dialogueUI.SetPortraitTalking(true);
        dialogueUI.PrepareDialogueText(currentFullLine);

        int characterCount = dialogueUI.GetDialogueCharacterCount();

        AudioClip voiceClip = dialogueData.voiceSound;
        float voicePitch = dialogueData.voicePitch;

        if (node.speaker == DialogueSpeaker.Player && DialogueState.Instance != null)
        {
            voiceClip = DialogueState.Instance.playerVoiceSound;
            voicePitch = DialogueState.Instance.playerVoicePitch;
        }

        for (int i = 0; i < characterCount; i++)
        {
            dialogueUI.RevealDialogueCharacter(i);

            char character = dialogueUI.GetDialogueCharacter(i);
            
            if (!char.IsWhiteSpace(character))
                SoundEffectManager.PlayVoice(voiceClip, voicePitch);

            float delay = dialogueData.typingSpeed;

            if (character == '.')
                delay += dialogueData.periodPause;

            yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;

        dialogueUI.SetPortraitTalking(false);

        AfterLineFinished();
    }

    private void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();

            dialogueUI.ShowAllDialogueText();
            dialogueUI.SetPortraitTalking(false);

            isTyping = false;

            AfterLineFinished();

            return;
        }

        if (isWaitingForChoice || isWaitingForName)
            return;

        StopAllCoroutines();

        DialogueNode currentNode = dialogueData.nodes[dialogueIndex];

        if (currentNode.endDialogue)
        {
            EndDialogue();
            return;
        }

        int nextIndex = GetNextNodeIndex(dialogueIndex);

        if (nextIndex < 0 || nextIndex >= dialogueData.nodes.Length)
        {
            EndDialogue();
            return;
        }

        dialogueIndex = nextIndex;

        DisplayCurrentNode();
    }

    private void AfterLineFinished()
    {
        DialogueNode node = dialogueData.nodes[dialogueIndex];

        dialogueUI.ShowContinueArrow(false);

        if (node.askForPlayerName)
        {
            isWaitingForName = true;
            dialogueUI.ShowNameInput(PlayerNameEntered);
            return;
        }

        if (DisplayChoices(node))
            return;

        if (node.autoProgress)
        {
            StartCoroutine(AutoProgress(node.autoProgressDelay));
            return;
        }

        dialogueUI.ShowContinueArrow(true);
    }

    private IEnumerator AutoProgress(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        NextLine();
    }

    private bool DisplayChoices(DialogueNode node)
    {
        if (node.choices == null || node.choices.Length == 0)
            return false;

        bool foundValidChoice = false;

        foreach (DialogueChoice choice in node.choices)
        {
            if (!ChoiceConditionsMet(choice))
                continue;

            foundValidChoice = true;
            DialogueChoice selectedChoice = choice;
            string displayedChoice = FormatText(choice.choiceText);
            dialogueUI.CreateChoiceButton(displayedChoice, () => ChooseOption(selectedChoice));
        }

        isWaitingForChoice = foundValidChoice;

        return foundValidChoice;
    }

    private void ChooseOption(DialogueChoice choice)
    {
        if (!string.IsNullOrWhiteSpace(choice.flagToSet) && DialogueState.Instance != null)
            DialogueState.Instance.SetFlag(choice.flagToSet);

        if (!string.IsNullOrWhiteSpace(choice.popupText) && NotificationManager.Instance != null)
            NotificationManager.Instance.ShowNotification(choice.popupText.Replace("{npcName}", dialogueData.npcName).Replace("{playerName}", DialogueState.Instance.PlayerName));

        dialogueUI.ShowContinueArrow(false);

        isWaitingForChoice = false;
        dialogueUI.ClearChoices();
        int nextIndex = choice.nextNodeIndex;

        if (nextIndex < 0)
            nextIndex = GetNextNodeIndex(dialogueIndex);

        if (nextIndex < 0 || nextIndex >= dialogueData.nodes.Length)
        {
            EndDialogue();
            return;
        }

        dialogueIndex = nextIndex;

        DisplayCurrentNode();
    }

    private void PlayerNameEntered(string playerName)
    {
        if (DialogueState.Instance != null)
            DialogueState.Instance.SetPlayerName(playerName);

        isWaitingForName = false;

        NextLine();
    }

    private bool ChoiceConditionsMet(DialogueChoice choice)
    {
        if (!string.IsNullOrWhiteSpace(choice.requiredFlag))
        {
            if (DialogueState.Instance == null)
                return false;

            if (!DialogueState.Instance.HasFlag(choice.requiredFlag))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(choice.forbiddenFlag))
        {
            if (DialogueState.Instance != null && DialogueState.Instance.HasFlag(choice.forbiddenFlag))
                return false;
        }

        return true;
    }

    private bool NodeConditionsMet(DialogueNode node)
    {
        if (!string.IsNullOrWhiteSpace(node.requiredFlag))
        {
            if (DialogueState.Instance == null)
                return false;

            if (!DialogueState.Instance.HasFlag(node.requiredFlag))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(node.forbiddenFlag))
        {
            if (DialogueState.Instance != null && DialogueState.Instance.HasFlag(node.forbiddenFlag))
                return false;
        }

        return true;
    }

    private int FindNextValidNode(int startIndex)
    {
        int index = startIndex;
        int safetyCounter = 0;

        while (index >= 0 && index < dialogueData.nodes.Length)
        {
            if (NodeConditionsMet(dialogueData.nodes[index]))
                return index;

            index = GetNextNodeIndex(index);

            safetyCounter++;
            if (safetyCounter > dialogueData.nodes.Length)
            {
                Debug.LogError("Dialogue appears to contain a loop.");
                return -1;
            }
        }

        return -1;
    }

    private int GetNextNodeIndex(int index)
    {
        DialogueNode node = dialogueData.nodes[index];

        if (node.nextNodeIndex >= 0)
            return node.nextNodeIndex;

        return index + 1;
    }

    private string FormatText(string text)
    {
        if (DialogueState.Instance != null)
            return DialogueState.Instance.FormatText(text);

        return text;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();

        dialogueUI.ShowContinueArrow(false);
        dialogueUI.SetPortraitTalking(false);
        dialogueUI.ClearDialogueText();

        isTyping = false;
        isDialogueActive = false;
        isWaitingForChoice = false;
        isWaitingForName = false;

        dialogueUI.ClearChoices();
        dialogueUI.HideNameInput();
        dialogueUI.ClearDialogueText();
        dialogueUI.ShowDialogue(false);

        PauseController.SetPause(false);
    }

    private void OnMouseDown()
    {
        if (!isDialogueActive)
            Interact();
    }
}