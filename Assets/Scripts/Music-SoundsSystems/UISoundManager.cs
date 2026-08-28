using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance { get; private set; }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlayClick();
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }
}