using System.Collections.Generic;
using UnityEngine;

public class DialogueState : MonoBehaviour
{
    public static DialogueState Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private string defaultPlayerName = "Player";

    public Sprite playerPortrait;
    public Sprite playerBlinkPortrait;
    public Sprite playerTalkingPortrait;
    public AudioClip playerVoiceSound;
    public float playerVoicePitch = 1f;

    public string PlayerName { get; private set; }

    private HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerName = defaultPlayerName;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return;

        PlayerName = newName.Trim();
    }

    public void SetFlag(string flag)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return;

        flags.Add(flag);

        Debug.Log("Dialogue flag unlocked: " + flag);
    }

    public void RemoveFlag(string flag)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return;

        flags.Remove(flag);
    }

    public bool HasFlag(string flag)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return false;

        return flags.Contains(flag);
    }

    public string FormatText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return text.Replace("{playerName}", PlayerName);
    }
}