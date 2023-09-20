using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    [SerializeField] public AudioMixer mixer;

    public const string MUSIC_KEY = "musicVolume";
    public const string SFX_KEY = "sfxVolume";

    // Start is called before the first frame update
    void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioManager transfers to all the scenes
        DontDestroyOnLoad(gameObject);

        InitSounds();

        LoadVolume();
    }

    void InitSounds()
    {
        // Add audiosource component to each audioclip
        // and initialize its properties
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = sound.mixer;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    // Play the desired sound giving its name
    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound not found: " +  name);
            return;
        }
        sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return;
        }
        sound.source.Stop();
    }

    // Load saved volume preferences
    void LoadVolume() // Volumes saved in SettingsMenu.cs
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        mixer.SetFloat(SettingsMenu.MIXER_MUSIC, Mathf.Log10((float)musicVolume) * 20);
        mixer.SetFloat(SettingsMenu.MIXER_SFX, Mathf.Log10((float)sfxVolume) * 20);
    }
}
