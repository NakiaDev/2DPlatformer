using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Allows object to break after depleting its "health".

[RequireComponent(typeof(RecoveryCounter))]

public class Breakable : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] Sprite _brokenSprite; //If destroyAfterDeath is false, a broken sprite will appear instead
    [SerializeField] GameObject _deathParticles;
    [SerializeField] bool _destroyAfterDeath = true; //If false, a broken sprite will appear instead of complete destruction
    int _health;
    [SerializeField] Instantiator _instantiator;
    [SerializeField] AudioClip _hitSound;
    [SerializeField] RecoveryCounter _recoveryCounter;
    [SerializeField] bool _requireDownAttack;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _recoveryCounter = GetComponent<RecoveryCounter>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void GetHurt(int hitPower)
    {
        //If breakable object health is above zero, it's not recovering from a recent hit, get hit!
        if (_health > 0 && !_recoveryCounter.Recovering && (!_requireDownAttack || (_requireDownAttack && NewPlayer.Instance.Pounding)))
        {
            if (NewPlayer.Instance.Pounding)
                NewPlayer.Instance.PoundEffect();

            if (_hitSound != null)
                GameManager.Instance.AudioSource.PlayOneShot(_hitSound);

            //Ensure the player can't hit the box multiple times in one hit
            _recoveryCounter.Counter = 0;

            StartCoroutine(NewPlayer.Instance.FreezeFrameEffect());

            _health -= hitPower;
            _animator.SetTrigger("hit");

            if (_health <= 0)
                Die();
        }
    }

    public void Die()
    {
        //Ensure timeScale is forced to 1 after breaking
        Time.timeScale = 1;

        //Activate deathParticles & unparent from this so they aren't destroyed!
        _deathParticles.SetActive(true);
        _deathParticles.transform.parent = null;

        if (_instantiator != null)
            _instantiator.InstantiateObjects();

        //Destroy me, or set my sprite to the brokenSprite
        if (_destroyAfterDeath)
            Destroy(gameObject);
        else
            _spriteRenderer.sprite = _brokenSprite;
    }
}
