using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip levelCompletedSound;
    public AudioClip failedLevelSound;
    public AudioClip heartPickSound;
    public AudioClip backgroundMusic;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.clip = backgroundMusic;
        _audioSource.Play();
    }


    public void PlayLevelCompleted()
    {
        _audioSource.PlayOneShot(levelCompletedSound);
    }

    public void PlayLevelFailed()
    {
        _audioSource.PlayOneShot(failedLevelSound);
    }

    public void PlayHeartPicked()
    {
        _audioSource.PlayOneShot(heartPickSound);
    }
}