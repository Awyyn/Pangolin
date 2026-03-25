using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource audioSource;
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // optional but recommended:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        ApplyVolume(savedVolume);
    }

    /*public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }*/
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        if (audioSource == null)
        {
            // try to recover
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                Debug.LogWarning("SFXManager: Missing AudioSource.");
                return;
            }
        }

        audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    // Called by OptionsManager
    public void ApplyVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }
}
