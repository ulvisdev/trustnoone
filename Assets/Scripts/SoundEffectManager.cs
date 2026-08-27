using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayVoice(AudioClip clip, float pitch)
    {
        if (clip == null || Instance == null)
            return;

        Instance.audioSource.pitch = pitch;
        Instance.audioSource.PlayOneShot(clip);
    }
}