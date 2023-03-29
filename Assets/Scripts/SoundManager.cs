using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Dictionary<string, AudioClip> soundDict = new ();
    private AudioSource audioSource;
    public AudioClip levelCompletedSound;
    public AudioClip failedLevelSound;
    private void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayLevelCompleted()
    {
        audioSource.PlayOneShot(levelCompletedSound);
    }
    
}