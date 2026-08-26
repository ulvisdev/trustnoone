using UnityEngine;

public class NarrationOnClick : MonoBehaviour
{
    public NarrationData narration;
    public bool triggerOnce;

    private bool hasTriggered;

    private void OnMouseDown()
    {
        if (triggerOnce && hasTriggered)
            return;

        if (NarrationController.Instance == null)
            return;

        bool started = NarrationController.Instance.StartNarration(narration);

        if (started && triggerOnce)
            hasTriggered = true;
    }
}