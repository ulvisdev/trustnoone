using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPauseMenuOpen { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Transitions")]
    [SerializeField] private MenuPanelTransition panelTransition;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool wasPausedBeforeMenu;

    private void Awake()
    {
        IsPauseMenuOpen = false;

        panelTransition.ShowImmediate(pausePanel, false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (panelTransition.IsBusy)
            return;

        if (ScreenFader.Instance != null && ScreenFader.Instance.IsBusy)
            return;

        if (IsPauseMenuOpen)
            Resume();
        else
            OpenPauseMenu();
    }

    public void OpenPauseMenu()
    {
        if (IsPauseMenuOpen)
            return;

        wasPausedBeforeMenu = PauseController.IsGamePaused;

        IsPauseMenuOpen = true;

        PauseController.SetPause(true);

        panelTransition.Show(pausePanel);
    }

    public void Resume()
    {
        if (!IsPauseMenuOpen)
            return;

        if (panelTransition.IsBusy)
            return;

        panelTransition.Hide(pausePanel, FinishResume);
    }

    private void FinishResume()
    {
        IsPauseMenuOpen = false;

        if (!wasPausedBeforeMenu)
            PauseController.SetPause(false);
    }

    public void GoToMainMenu()
    {
        if (ScreenFader.Instance == null)
            return;

        if (ScreenFader.Instance.IsBusy)
            return;

        IsPauseMenuOpen = false;

        ScreenFader.Instance.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        IsPauseMenuOpen = false;

        PauseController.SetPause(false);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        IsPauseMenuOpen = false;
    }
}