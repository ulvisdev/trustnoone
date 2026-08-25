using UnityEngine;

public class InspectableItem : MonoBehaviour
{
    [Header("Inspection")]
    [Tooltip("Flag that gets unlocked when this object is inspected.")]
    public string flagToSet;

    [Tooltip("If true, inspecting the object again will not trigger its popup again.")]
    public bool onlyTriggerOnce = true;

    [Header("Popup")]
    public bool showPopup = true;

    [TextArea(2, 4)]
    public string popupText = "New clue discovered.";

    private void OnMouseDown()
    {
        if (PauseController.IsGamePaused)
            return;

        Inspect();
    }

    private void Inspect()
    {
        if (DialogueState.Instance == null)
            return;

        bool alreadyInspected = DialogueState.Instance.HasFlag(flagToSet);

        if (!string.IsNullOrWhiteSpace(flagToSet))
            DialogueState.Instance.SetFlag(flagToSet);

        if (showPopup && NotificationManager.Instance != null && (!onlyTriggerOnce || !alreadyInspected))
            NotificationManager.Instance.ShowNotification(popupText);
    }
}