using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Attach this to any collectable. When it is instantiated from a broken box or dead enemy, 
it will launch. This script also ensures the collectable's trigger is disabled for
a brief period so the player doesn't immediately collect it after instantiation, not knowing what he collected.
*/

public class Ejector : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _bounceSound;
    [SerializeField] BoxCollider2D _collectableTrigger;
    float _counter; //Counts to a value, and then allows the collectable can be collected
    public bool LaunchOnStart;
    Vector2 _launchPower = new Vector2(300, 300);
    Rigidbody2D _rb;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        if (LaunchOnStart)
        {
            Launch(_launchPower);
            _collectableTrigger.enabled = false;
        }
        else
        {
            _rb.isKinematic = true;
            GetComponent<Collider2D>().enabled = false;
            _collectableTrigger.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_collectableTrigger != null)
        {
            if (_counter > .5f)
                _collectableTrigger.enabled = true;
            else
                _counter += Time.deltaTime;
        }
    }

    //Called when the cube hits the floor
    void OnCollisionEnter2D(Collision2D col)
    {
        if (LaunchOnStart && _collectableTrigger.enabled)
            _audioSource.PlayOneShot(_bounceSound, _rb.velocity.magnitude / 10 * _audioSource.volume);
    }

    public void Launch(Vector2 launchPower)
    {
        //Launch collectable after box explosion at the specificied launch power, multiplied by a random range.
        _rb.AddForce(new Vector2(launchPower.x * Random.Range(-1f, 1f), launchPower.y * Random.Range(1f, 1.5f)));
    }
}
