using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();

                if (instance == null)
                {
                    GameObject soundManagerObj = new GameObject("SoundManager");
                    instance = soundManagerObj.AddComponent<SoundManager>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAuidoSource();
    }

    private void InitializeAuidoSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlayBackgroundLoginMusic()
    {
        if (audioClips.Length > 0)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }
    }

    public void PlayBackgroundLobbyMusic()
    {
        if (audioClips.Length > 1)
        {
            audioSource.clip = audioClips[1];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayButtonAndClickSound()
    {
        if (audioClips.Length > 2)
            audioSource.PlayOneShot(audioClips[2]);
    }

    public void PlayVictorySound()
    {
        if (audioClips.Length > 3)
            audioSource.PlayOneShot(audioClips[3]);
    }
    public void StopMusic() => audioSource.Stop();
}
