using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }
    public static bool IsGamePaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public static void SetPause(bool paused)
    {
        IsGamePaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
}