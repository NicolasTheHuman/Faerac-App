using UnityEngine;

public class SoundInstance : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    public void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(_audioClip);
    }
    
    public void PlaySFX(float volume)
    {
        AudioManager.Instance.PlaySFX(_audioClip, volume);
    }
}
