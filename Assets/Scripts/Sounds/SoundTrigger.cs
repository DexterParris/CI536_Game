using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public string triggerName;
    public AudioClip[] audioClips;


    public void TriggerSounds()
    {
        playSoundHere();
    }


    void playSoundHere()
    {
        // Pick a random sound from the array
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];

        // Play the sound with random pitch variation
        GameObject soundObject = new GameObject("OneShotSound");
        soundObject.transform.position = transform.position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = randomClip;
        audioSource.volume = 0.5f;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
        Destroy(soundObject, randomClip.length);

    }
}
