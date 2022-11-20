using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnimatorFunctions : MonoBehaviour
{
    public List<SoundTuple> sounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaySound(string name)
    {
        if (sounds == null || sounds.Count == 0)
        {
            Debug.Log("sounds list empty");
            return;
        }

        SoundTuple playSound = sounds.FirstOrDefault(s => s.name == name);
        if (playSound == null)
        {
            Debug.Log("SoundTuple not found: " + name);
            return;
        }

        if (playSound.audioClip.Length == 0)
        {
            Debug.Log("No audio clips added at: " + name);
            return;
        }

        if (playSound.audioClip.Length == 1)
            NewPlayer.Instance.sfxAudioSource.PlayOneShot(playSound.audioClip[0], playSound.volume * UnityEngine.Random.Range(.8f, 1.4f));
        else
            NewPlayer.Instance.sfxAudioSource.PlayOneShot(playSound.audioClip[UnityEngine.Random.Range(0, playSound.audioClip.Length)], playSound.volume * UnityEngine.Random.Range(.8f, 1.4f));
    }

}

[Serializable]
public class SoundTuple
{
    public string name;
    public AudioClip[] audioClip;
    [Range(0f, 1f)]
    public float volume;
}
