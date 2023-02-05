using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RecoveryCounter))]

public class EnemyBase : MonoBehaviour
{
    [Header("Reference")]
    AudioSource _audioSource;
    Animator _animator;
    [SerializeField] Instantiator _instantiator;
    RecoveryCounter _recoveryCounter;

    [Header("Properties")]
    [SerializeField] GameObject _deathParticles;
    [SerializeField] int _health = 3;
    [SerializeField] AudioClip _hitSound;
    [SerializeField] bool _requirePoundAttack; //Requires the player to use the down attack to hurt

    // Properties
    public AudioSource AudioSource { get { return _audioSource; } }
    public Animator Animator { get { return _animator; } }
    public RecoveryCounter RecoveryCounter { get { return _recoveryCounter; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _recoveryCounter = GetComponent<RecoveryCounter>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (_health <= 0)
            Die();
    }

    public void GetHurt(int launchDirection, int hitPower)
    {
        //Hit the enemy, causing a damage effect, and decreasing health. Allows for requiring a downward pound attack
        if (GetComponent<Walker>() != null && !_recoveryCounter.Recovering)
        {
            if (!_requirePoundAttack || (_requirePoundAttack && NewPlayer.Instance.Pounding))
            {
                NewPlayer.Instance.CameraEffects.Shake(100, 1);
                _health -= hitPower;
                _animator.SetTrigger("hurt");

                _audioSource.pitch = (1);
                _audioSource.PlayOneShot(_hitSound);

                //Ensure the enemy and also the player cannot engage in hitting each other for the max recoveryTime for each
                _recoveryCounter.Counter = 0;
                NewPlayer.Instance.RecoveryCounter.Counter = 0;

                if (NewPlayer.Instance.Pounding)
                {
                    NewPlayer.Instance.PoundEffect();
                }

                if (GetComponent<Walker>() != null)
                {
                    Walker walker = GetComponent<Walker>();
                    walker.Launch = launchDirection * walker.HurtLaunchPower / 5;
                    walker.velocity.y = walker.HurtLaunchPower;
                    walker.DirectionSmooth = launchDirection;
                    walker.Direction = walker.DirectionSmooth;
                }

                NewPlayer.Instance.FreezeFrameEffect();
            }
        }
    }

    public void Die()
    {
        if (NewPlayer.Instance.Pounding)
        {
            NewPlayer.Instance.PoundEffect();
        }

        NewPlayer.Instance.CameraEffects.Shake(200, 1);
        _health = 0;
        _deathParticles.SetActive(true);
        _deathParticles.transform.parent = transform.parent;
        _instantiator.InstantiateObjects();
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}