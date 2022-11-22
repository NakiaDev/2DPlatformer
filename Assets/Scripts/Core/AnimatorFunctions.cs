using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AnimatorFunctions : MonoBehaviour
{
    public List<Particles> particlesList;
    public List<Sounds> sounds;

    void PlaySound(string name)
    {
        if (NewPlayer.Instance == null) return;

        if (sounds == null || sounds.Count == 0)
        {
            Debug.Log("sounds list empty");
            return;
        }

        Sounds playSound = sounds.FirstOrDefault(s => s.name == name);
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

    public void EmitParticles(string name)
    {
        if (particlesList == null || particlesList.Count == 0)
        {
            Debug.Log("Particles list empty");
            return;
        }

        Particles particles = particlesList.FirstOrDefault(p => p.name == name);
        if (particles == null)
        {
            Debug.Log("Particles not found: " + name);
        }

        particles.particleSystem.Emit(particles.emitAmount);
    }
}

[Serializable]
public class Sounds
{
    public string name;
    public AudioClip[] audioClip;
    [Range(0f, 1f)]
    public float volume;
}

[Serializable]
public class Particles
{
    public string name;
    public ParticleSystem particleSystem;
    [Range(0, 100)]
    public int emitAmount;
}
