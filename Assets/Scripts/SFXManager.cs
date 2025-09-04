using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource audioSource;
    public AudioMixer audioMixer;

    public Slider sfxSlider;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (sfxSlider != null)
        {
            float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

            sfxSlider.SetValueWithoutNotify(savedSFXVolume); // set slider visually
            SFXManager.instance.SetVolume(savedSFXVolume);  // apply saved volume

            sfxSlider.onValueChanged.AddListener(SFXManager.instance.SetVolume); // future changes
        }
    }


    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // set volume in linear 0-1 scale
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.PlayOneShot(clip);
    }

    public void SetVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}
