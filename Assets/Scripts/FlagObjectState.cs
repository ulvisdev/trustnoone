using UnityEngine;

public class FlagObjectState : MonoBehaviour
{
    [Header("Condition")]
    [SerializeField] private string requiredFlag;
    [SerializeField] private string forbiddenFlag;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool show = true;

        if (!string.IsNullOrEmpty(requiredFlag) && !DialogueState.Instance.HasFlag(requiredFlag))
            show = false;

        if (!string.IsNullOrEmpty(forbiddenFlag) && DialogueState.Instance.HasFlag(forbiddenFlag))
            show = false;

        gameObject.SetActive(show);
    }
}