using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Notification")]
    public GameObject notificationPrefab;
    public Transform notificationContainer;

    [Header("Timing")]
    public float displayTime = 3f;
    public float fadeTime = 0.2f;

    [Header("Movement")]
    public float slideDistance = 20f;

    [Header("Stack")]
    public bool newestOnTop = true;

    // private void Awake()
    // {
    //     if (Instance == null)
    //         Instance = this;
    //     else
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    // }

    public void ShowNotification(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        GameObject notificationObject = Instantiate(notificationPrefab, notificationContainer);

        if (newestOnTop)
            notificationObject.transform.SetAsFirstSibling();

        NotificationItem notificationItem = notificationObject.GetComponent<NotificationItem>();

        if (notificationItem == null)
        {
            Debug.LogError("Notification prefab is missing NotificationItem.");
            Destroy(notificationObject);
            return;
        }

        notificationItem.Show(message, displayTime, fadeTime, slideDistance);
    }
}