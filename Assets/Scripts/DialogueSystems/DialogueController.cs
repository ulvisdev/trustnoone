using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    [Header("Main")]
    public GameObject dialoguePanel;

    [Header("NPC Dialogue Box")]
    public GameObject npcDialogueBox;
    public TMP_Text npcDialogueText;
    public TMP_Text npcNameText;
    public Image npcPortraitImage;

    [Header("Player Dialogue Box")]
    public GameObject playerDialogueBox;
    public TMP_Text playerDialogueText;
    public TMP_Text playerNameText;
    public Image playerPortraitImage;

    [Header("Choices")]
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    [Header("Player Name Input")]
    public GameObject nameInputPanel;
    public TMP_InputField playerNameInput;
    public Button confirmNameButton;

    [Header("Text Effects")]
    public DialogueTextEffects npcTextEffects;
    public DialogueTextEffects playerTextEffects;

    [Header("Portrait Animation")]
    public DialoguePortraitAnimator npcPortraitAnimator;
    public DialoguePortraitAnimator playerPortraitAnimator;

    [Header("Continue Arrows")]
    public GameObject npcContinueArrow;
    public GameObject playerContinueArrow;

    private DialogueTextEffects activeTextEffects;
    private DialoguePortraitAnimator activePortraitAnimator;
    private GameObject activeContinueArrow;

    private TMP_Text activeDialogueText;

    private Action<string> nameConfirmedCallback;

    // private void Awake()
    // {
    //     if (Instance == null)
    //         Instance = this;
    //     else
    //         Destroy(gameObject);
    // }

    private void Start()
    {
        ShowDialogue(false);

        if (nameInputPanel != null)
            nameInputPanel.SetActive(false);

        if (confirmNameButton != null)
            confirmNameButton.onClick.AddListener(ConfirmPlayerName);
    }

    public void ShowDialogue(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    public void SetSpeaker(DialogueSpeaker speaker, NPCDialogue npcData)
    {
        ShowContinueArrow(false);

        if (speaker == DialogueSpeaker.NPC)
        {
            npcDialogueBox.SetActive(true);
            playerDialogueBox.SetActive(false);

            npcNameText.text = npcData.npcName;
            npcPortraitImage.sprite = npcData.portrait;

            activeDialogueText = npcDialogueText;
            activeTextEffects = npcTextEffects;
            activePortraitAnimator = npcPortraitAnimator;
            activeContinueArrow = npcContinueArrow;

            if (npcPortraitAnimator != null)
                npcPortraitAnimator.Configure(npcData.portrait, npcData.blinkPortrait, npcData.talkingPortrait);
        }
        else
        {
            npcDialogueBox.SetActive(false);
            playerDialogueBox.SetActive(true);

            activeDialogueText = playerDialogueText;
            activeTextEffects = playerTextEffects;
            activePortraitAnimator = playerPortraitAnimator;
            activeContinueArrow = playerContinueArrow;

            if (DialogueState.Instance != null)
            {
                playerNameText.text = DialogueState.Instance.PlayerName;
                playerPortraitImage.sprite = DialogueState.Instance.playerPortrait;

                if (playerPortraitAnimator != null)
                    playerPortraitAnimator.Configure(DialogueState.Instance.playerPortrait, DialogueState.Instance.playerBlinkPortrait, DialogueState.Instance.playerTalkingPortrait);
            }
            else
            {
                playerNameText.text = "Player";
            }
        }
    }

    public void SetDialogueText(string text)
    {
        if (activeDialogueText != null)
            activeDialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();

        buttonText.text = choiceText;

        Button button = choiceButton.GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    public void ShowNameInput(Action<string> callback)
    {
        nameConfirmedCallback = callback;

        nameInputPanel.SetActive(true);

        playerNameInput.text = "";
        playerNameInput.ActivateInputField();
    }

    public void HideNameInput()
    {
        if (nameInputPanel != null)
            nameInputPanel.SetActive(false);

        nameConfirmedCallback = null;
    }

    private void ConfirmPlayerName()
    {
        string enteredName = playerNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(enteredName))
            return;

        nameInputPanel.SetActive(false);

        Action<string> callback = nameConfirmedCallback;
        nameConfirmedCallback = null;

        callback?.Invoke(enteredName);
    }

    public void PrepareDialogueText(string text)
    {
        if (activeTextEffects != null)
            activeTextEffects.PrepareText(text);
        else if (activeDialogueText != null)
            activeDialogueText.text = text;
    }

    public int GetDialogueCharacterCount()
    {
        if (activeTextEffects == null)
            return 0;

        return activeTextEffects.GetCharacterCount();
    }

    public char GetDialogueCharacter(int index)
    {
        if (activeTextEffects == null)
            return ' ';

        return activeTextEffects.GetCharacter(index);
    }

    public bool IsDialogueCharacterVisible(int index)
    {
        if (activeTextEffects == null)
            return false;

        return activeTextEffects.IsCharacterVisible(index);
    }

    public void RevealDialogueCharacter(int index)
    {
        if (activeTextEffects != null)
            activeTextEffects.RevealCharacter(index);
    }

    public void ShowAllDialogueText()
    {
        if (activeTextEffects != null)
            activeTextEffects.ShowAll();
    }

    public void ClearDialogueText()
    {
        if (activeTextEffects != null)
            activeTextEffects.Clear();
        else if (activeDialogueText != null)
            activeDialogueText.text = "";
    }

    public void SetPortraitTalking(bool talking)
    {
        if (activePortraitAnimator != null)
            activePortraitAnimator.SetTalking(talking);
    }

    public void ShowContinueArrow(bool show)
    {
        if (npcContinueArrow != null)
            npcContinueArrow.SetActive(false);

        if (playerContinueArrow != null)
            playerContinueArrow.SetActive(false);

        if (show && activeContinueArrow != null)
            activeContinueArrow.SetActive(true);
    }
}