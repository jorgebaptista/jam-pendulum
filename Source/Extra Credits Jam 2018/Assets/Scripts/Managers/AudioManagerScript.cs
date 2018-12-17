using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Music,
    Sfx,
    Player,
    Enemies
}

[System.Serializable]
public class Sound
{
    [Tooltip("Name of the sound file that will be called from other gameobjects. Must be written correctly.")]
    public string name;
    [Tooltip("Drag and drop sounds here. If more than one sound is added the audio source will choose one randomly when called.")]
    public AudioClip[] soundFile;
    public AudioType type;

    private AudioSource audioSource;

    [Space]
    [Tooltip("Randomize volume? If unchecked Min Volume variable will be ignored.")]
    public bool randomizeVol;
    [Range(0, 1)]
    public float volume = 0.7f;
    [Range(0.4f, 1)]
    public float minVolume = 0.5f;

    [Space]
    [Tooltip("Randomize volume? If unchecked Min Volume variable will be ignored.")]
    public bool randomizePitch;
    [Range(0.5f, 1.5f)]
    public float pitch = 1;
    [Range(0.5f, 1.5f)]
    public float minPitch = 0.5f;

    public void SetAudioSource(AudioSource _audioSource)
    {
        audioSource = _audioSource;
    }

    public void Play(AudioSource source = null)
    {
        float _volume = randomizeVol ? Random.Range(minVolume, volume) : volume;
        if (!source) audioSource.pitch = randomizePitch ? Random.Range(minPitch, pitch) : pitch;
        else source.pitch = randomizePitch ? Random.Range(minPitch, pitch) : pitch;

        if (!source) audioSource.PlayOneShot(soundFile[Random.Range(0, soundFile.Length)], _volume);
        else source.PlayOneShot(soundFile[Random.Range(0, soundFile.Length)], _volume);
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}

public class AudioManagerScript : MonoBehaviour
{
    [Header("Audio Source References")]
    [Space]
    [SerializeField]
    private AudioSource musicAudioSource;
    [SerializeField]
    private AudioSource sFXAudioSource, playerAudioSource, enemiesAudioSource;

    [Header("Sounds")]
    [Space]
    [SerializeField]
    private Sound[] sounds;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            switch (sound.type)
            {
                case AudioType.Music:
                    sound.SetAudioSource(musicAudioSource);
                    break;
                case AudioType.Sfx:
                    sound.SetAudioSource(sFXAudioSource);
                    break;
                case AudioType.Player:
                    sound.SetAudioSource(playerAudioSource);
                    break;
                case AudioType.Enemies:
                    sound.SetAudioSource(enemiesAudioSource);
                    break;
                default:
                    Debug.LogError("Type " + sound.type + " not found.");
                    break;
            }
        }
    }

    public void PlaySound(string soundName, string requestAuthor)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == soundName)
            {
                sound.Play();
                return;
            }
        }
        Debug.LogError(soundName + " not found, please name the sounds correctly. Source: " + requestAuthor + ".");
    }
    public void PlaySound(string soundName, string requestAuthor, AudioSource source)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == soundName)
            {
                sound.Play(source);
                return;
            }
        }
        Debug.LogError(soundName + " not found, please name the sounds correctly. Source: " + requestAuthor + ".");
    }

    public void StopSound(string soundName)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.name == soundName)
            {
                sound.Stop();
                return;
            }
        }
        Debug.LogError(soundName + " not found, please name the sounds correctly.");
    }

    public void StopAllSounds()
    {
        musicAudioSource.Stop();
        sFXAudioSource.Stop();
        playerAudioSource.Stop();
        enemiesAudioSource.Stop();
    }
}
