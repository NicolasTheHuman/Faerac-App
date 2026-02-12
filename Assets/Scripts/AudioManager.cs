using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private bool _mute;
    
    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
        
        
        _musicSource = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();

        _musicSource.loop = true;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if(_mute)
            return;
        
        _musicSource.volume = 1f;
        _musicSource.clip = musicClip;
        _musicSource.Play();
    }

    public void PlayMusic(AudioClip musicClip, float volume)
    {
        if(_mute)
            return;
        
        _musicSource.clip = musicClip;
        _musicSource.volume = volume;
        _musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if(_mute)
            return;
        
        _sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if(_mute)
            return;
        
        _sfxSource.PlayOneShot(clip, volume);
    }
    

    public void StopSFX(AudioClip clip)
    {
        if (_sfxSource.clip == clip)
            _sfxSource.Stop();
    }

    public void ToggleMute()
    {
        _mute = !_mute;
    }
}