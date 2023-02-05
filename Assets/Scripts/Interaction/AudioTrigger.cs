using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Use on any gameObject to fade in and out an audioClip. Used for things like
ambience and music.*/

public class AudioTrigger : MonoBehaviour
{

    AudioSource _audioSource;
    [SerializeField] bool _autoPlay; //Begins playing sound immediately without the player triggering the collider
    [SerializeField] bool _controlsTitle; //This allows the level title to fade in while also fading in the music
    [SerializeField] float _fadeSpeed; //How fast do we increase the volume? If set to 0, it will just play at a volume of 1
    [SerializeField] bool _loop;
    [SerializeField] AudioClip _sound;
    [Range(0f, 1f)]
    public float MaxVolume; //The volume we are going to fade to
    bool _triggered; //Is set to true once the player touches the collider trigger zone

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {
        if (_fadeSpeed != 0) _audioSource.volume = 1;
        _audioSource.clip = _sound;
        StartCoroutine(EnableCollider());
    }

    // Update is called once per frame
    void Update()
    {
        _audioSource.loop = _loop;

        /*If the player isn't dead, and we either trigger or want to 
        AudioTrigger to automatically play, the audioSource will begin playing.
        */

        if (!NewPlayer.Instance.Dead)
        {
            if (_triggered || _autoPlay)
            {
                if (!_audioSource.isPlaying)
                    _audioSource.Play();

                //Begin fading in the audioSource volume as long as it's smaller than the goToVolume
                if (_audioSource.volume < MaxVolume)
                    _audioSource.volume += _fadeSpeed * Time.deltaTime;
            }
            else
            {
                if (_audioSource.volume > 0)
                    _audioSource.volume -= _fadeSpeed * Time.deltaTime;
                else
                    _audioSource.Stop();
            }
        }
        else
            _audioSource.Stop();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == NewPlayer.Instance.gameObject && !_triggered)
        {
            if (_controlsTitle)
                GameManager.Instance.Hud.Animator.SetBool("showTitle", true);

            _triggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col == NewPlayer.Instance)
            _triggered = false;
    }

    private IEnumerator EnableCollider()
    {
        /*If the player spawns inside a large trigger area, it won't trigger. Therefore, we wait 4 seconds 
        to actually enable it so the trigger can actually occur  */
        yield return new WaitForSeconds(4f);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}