using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public CutsceneData cutscene;
    public bool playOnStart;

    private void Start()
    {
        if (playOnStart)
            PlayCutscene();
    }

    public void PlayCutscene()
    {
        if (cutscene == null)
            return;

        CutsceneController.Instance.StartCutscene(cutscene);
    }

    private void OnMouseDown()
    {
        if (!playOnStart)
            PlayCutscene();
    }
}