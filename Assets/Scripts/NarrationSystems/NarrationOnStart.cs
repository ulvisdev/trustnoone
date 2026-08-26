using System.Collections;
using UnityEngine;

public class NarrationOnStart : MonoBehaviour
{
    public NarrationData narration;

    private IEnumerator Start()
    {
        yield return null;

        if (NarrationController.Instance != null)
            NarrationController.Instance.StartNarration(narration);
    }
}