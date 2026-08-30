using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void OnMouseDown()
    {
        GoToRoom();
    }

    public void GoToRoom()
    {
        if (ScreenFader.Instance == null)
            return;

        if (ScreenFader.Instance.IsBusy)
            return;

        ScreenFader.Instance.LoadScene(sceneName);
    }

}