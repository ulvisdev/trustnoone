using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Transitions")]
    [SerializeField] private MenuPanelTransition panelTransition;

    [SerializeField] private UIMenuEntrance menuEntrance;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";

    private void Awake()
    {
        PauseController.SetPause(false);

        panelTransition.ShowImmediate(mainPanel, true);
        panelTransition.ShowImmediate(howToPlayPanel, false);
        panelTransition.ShowImmediate(optionsPanel, false);
        panelTransition.ShowImmediate(creditsPanel, false);
    }

    private void Start()
    {
        menuEntrance.Play();
    }

    public void PlayGame()
    {
        if (ScreenFader.Instance == null)
            return;

        ScreenFader.Instance.LoadScene(gameSceneName);
    }

    public void OpenHowToPlay()
    {
        panelTransition.Switch(mainPanel, howToPlayPanel);
    }

    public void CloseHowToPlay()
    {
        panelTransition.Switch(howToPlayPanel, mainPanel);
    }

    public void OpenOptions()
    {
        panelTransition.Switch(mainPanel, optionsPanel);
    }

    public void CloseOptions()
    {
        panelTransition.Switch(optionsPanel, mainPanel);
    }

    public void OpenCredits()
    {
        panelTransition.Switch(mainPanel, creditsPanel);
    }

    public void CloseCredits()
    {
        panelTransition.Switch(creditsPanel, mainPanel);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}