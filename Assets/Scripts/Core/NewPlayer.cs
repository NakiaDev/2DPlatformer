using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RecoveryCounter))]

public class NewPlayer : PhysicsObject
{
    [Header("References")]
    AudioSource _audioSource;
    Animator _animator;
    GameObject _attackHit;
    [System.NonSerialized] public CameraEffects CameraEffects;
    [SerializeField] ParticleSystem _deathParticles;
    [SerializeField] GameObject _graphic;
    [SerializeField] Component[] _graphicSprites;
    [SerializeField] ParticleSystem _jumpParticles;
    [SerializeField] GameObject _pauseMenu;
    RecoveryCounter _recoveryCounter;

    [Header("Attributes")]
    bool _dead = false;
    bool _frozen = false;
    float _fallForgivenessCounter; //Counts how long the player has fallen off a ledge
    [SerializeField] float _fallForgiveness = .2f; //How long the player can fall from a ledge and still jump
    enum GroundTypes { Ground }
    GroundTypes _groundType = GroundTypes.Ground;
    RaycastHit2D _ground;
    [SerializeField] Vector2 _hurtLaunchPower; //How much force should be applied to the player when getting hurt?
    float _launch; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    [SerializeField] float _launchRecovery; //How slow should recovering from the launch be? (Higher the number, the longer the launch will last)
    float _maxSpeed = 7; //Max move speed
    float _jumpPower = 17;
    bool _jumping;
    Vector3 _origLocalScale;
    bool _pounded;
    bool _pounding;

    [Header("Inventory")]
    [SerializeField] public int Coins;
    [SerializeField] public int Health;
    [SerializeField] int _maxHealth = 100;

    [Header("Sounds")]
    [SerializeField] AudioClip _deathSound;
    [SerializeField] AudioClip _equipSound;
    [SerializeField] AudioClip _grassSound;
    [SerializeField] AudioClip _hurtSound;
    [SerializeField] AudioClip[] _hurtSounds;
    [SerializeField] AudioClip _holsterSound;
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _landSound;
    [SerializeField] AudioClip _poundSound;
    [SerializeField] AudioClip _punchSound;
    [SerializeField] AudioClip[] _poundActivationSounds;
    [SerializeField] AudioClip _outOfAmmoSound;
    [SerializeField] AudioClip _stepSound;
    int _whichHurtSound;

    // Properties
    public bool Pounding { get { return _pounding; } }
    public bool Frozen { get { return _frozen; } }
    public int MaxHealth { get { return _maxHealth; } }
    public AudioSource AudioSource { get { return _audioSource; } }
    public RecoveryCounter RecoveryCounter { get { return _recoveryCounter; } }
    public bool Dead { get { return _dead; } }

    // singleton because of the singleplayer mode
    private static NewPlayer instance;
    public static NewPlayer Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<NewPlayer>();
            return instance;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _recoveryCounter = GetComponent<RecoveryCounter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Health = _maxHealth;
        _origLocalScale = transform.localScale;

        //Find all sprites so we can hide them when the player dies.
        _graphicSprites = GetComponentsInChildren<SpriteRenderer>();

        SetGroundType();
    }

    public void SetGroundType()
    {
        //If we want to add variable ground types with different sounds, it can be done here
        switch (_groundType)
        {
            case GroundTypes.Ground:
                _stepSound = _grassSound;
                break;
        }
    }

    // TODO: REVERT FIXEDUPDATE TO NORMAL UPDATE
    private void FixedUpdate()
    {
        // Player movement & attack
        Vector2 move = Vector2.zero;
        _ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up);

        //Lerp launch back to zero at all times
        _launch += (0 - _launch) * Time.deltaTime * _launchRecovery;

        if (Input.GetButtonDown("Cancel"))
            _pauseMenu.SetActive(true);

        //Movement, jumping, and attacking!
        if (!_frozen)
        {
            move.x = Input.GetAxis("Horizontal") + _launch;

            if (Input.GetButtonDown("Jump") && _animator.GetBool("grounded") == true && !_jumping)
            {
                _animator.SetBool("pounded", false);
                Jump(1f);
            }

            //Flip the graphic's localScale
            if (move.x > 0.01f)
                _graphic.transform.localScale = new Vector3(_origLocalScale.x, transform.localScale.y, transform.localScale.z);
            else if (move.x < -0.01f)
                _graphic.transform.localScale = new Vector3(-_origLocalScale.x, transform.localScale.y, transform.localScale.z);

            //Punch
            if (Input.GetMouseButtonDown(0))
                _animator.SetTrigger("attack");


            //Allow the player to jump even if they have just fallen off an edge ("fall forgiveness")
            if (!grounded)
            {
                if (_fallForgivenessCounter < _fallForgiveness && !_jumping)
                    _fallForgivenessCounter += Time.deltaTime;
                else
                    _animator.SetBool("grounded", false);
            }
            else
            {
                _fallForgivenessCounter = 0;
                _animator.SetBool("grounded", true);
            }

            //Set each animator float, bool, and trigger to it knows which animation to fire
            _animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / _maxSpeed);
            _animator.SetFloat("velocityY", velocity.y);
            _animator.SetInteger("attackDirectionY", (int)Input.GetAxis("VerticalDirection"));
            _animator.SetInteger("moveDirection", (int)Input.GetAxis("HorizontalDirection"));
            targetVelocity = move * _maxSpeed;
        }
        else
        {
            //If the player is set to frozen, his launch should be zeroed out!
            _launch = 0;
        }
    }

    public void Freeze(bool freeze)
    {
        //Set all animator params to ensure the player stops running, jumping, etc and simply stands
        if (freeze)
        {
            _animator.SetInteger("moveDirection", 0);
            _animator.SetBool("grounded", true);
            _animator.SetFloat("velocityX", 0f);
            _animator.SetFloat("velocityY", 0f);
            GetComponent<PhysicsObject>().targetVelocity = Vector2.zero;
        }

        _frozen = freeze;
        _launch = 0;
    }

    public void GetHurt(int hurtDirection, int hitPower)
    {
        //If the player is not frozen (ie talking, spawning, etc), recovering, and pounding, get hurt!
        if (!_frozen && !_recoveryCounter.Recovering && !_pounding)
        {
            HurtEffect();
            CameraEffects.Shake(100, 1);
            _animator.SetTrigger("hurt");
            velocity.y = _hurtLaunchPower.y;
            _launch = hurtDirection * _hurtLaunchPower.x;
            _recoveryCounter.Counter = 0;

            if (Health <= 0)
                StartCoroutine(Die());
            else
                Health -= hitPower;

            GameManager.Instance.Hud.HealthBarHurt();
        }
    }

    private void HurtEffect()
    {
        GameManager.Instance.AudioSource.PlayOneShot(_hurtSound);
        StartCoroutine(FreezeFrameEffect());
        GameManager.Instance.AudioSource.PlayOneShot(_hurtSounds[_whichHurtSound]);

        if (_whichHurtSound >= _hurtSounds.Length - 1)
            _whichHurtSound = 0;
        else
            _whichHurtSound++;

        CameraEffects.Shake(100, 1f);
    }

    public IEnumerator FreezeFrameEffect(float length = .007f)
    {
        Time.timeScale = .1f;
        yield return new WaitForSeconds(length);
        Time.timeScale = 1f;
    }


    public IEnumerator Die()
    {
        if (!_frozen)
        {
            _dead = true;
            _deathParticles.Emit(10);
            GameManager.Instance.AudioSource.PlayOneShot(_deathSound);
            Hide(true);
            Time.timeScale = .6f;
            yield return new WaitForSeconds(5f);
            GameManager.Instance.Hud.Animator.SetTrigger("coverScreen");
            GameManager.Instance.Hud.LoadSceneName = SceneManager.GetActiveScene().name;
            Time.timeScale = 1f;
        }
    }

    public void ResetLevel()
    {
        Freeze(true);
        _dead = false;
        Health = _maxHealth;
    }

    public void Jump(float jumpMultiplier)
    {
        if (velocity.y != _jumpPower)
        {
            velocity.y = _jumpPower * jumpMultiplier; //The jumpMultiplier allows us to use the Jump function to also launch the player from bounce platforms
            PlayJumpSound();
            PlayStepSound();
            JumpEffect();
            _jumping = true;
        }
    }

    public void PlayStepSound()
    {
        //Play a step sound at a random pitch between two floats, while also increasing the volume based on the Horizontal axis
        _audioSource.pitch = (Random.Range(0.9f, 1.1f));
        _audioSource.PlayOneShot(_stepSound, Mathf.Abs(Input.GetAxis("Horizontal") / 10));
    }

    public void PlayJumpSound()
    {
        _audioSource.pitch = (Random.Range(1f, 1f));
        GameManager.Instance.AudioSource.PlayOneShot(_jumpSound, .1f);
    }


    public void JumpEffect()
    {
        _jumpParticles.Emit(1);
        _audioSource.pitch = (Random.Range(0.6f, 1f));
        _audioSource.PlayOneShot(_landSound);
    }

    public void LandEffect()
    {
        if (_jumping)
        {
            _jumpParticles.Emit(1);
            _audioSource.pitch = (Random.Range(0.6f, 1f));
            _audioSource.PlayOneShot(_landSound);
            _jumping = false;
        }
    }

    public void PunchEffect()
    {
        GameManager.Instance.AudioSource.PlayOneShot(_punchSound);
        CameraEffects.Shake(100, 1f);
    }

    public void ActivatePound()
    {
        //A series of events needs to occur when the player activates the pound ability
        if (!_pounding)
        {
            _animator.SetBool("pounded", false);

            if (velocity.y <= 0)
            {
                velocity = new Vector3(velocity.x, _hurtLaunchPower.y / 2, 0.0f);
            }

            GameManager.Instance.AudioSource.PlayOneShot(_poundActivationSounds[Random.Range(0, _poundActivationSounds.Length)]);
            _pounding = true;
            FreezeFrameEffect(.3f);
        }
    }
    public void PoundEffect()
    {
        //As long as the player as activated the pound in ActivatePound, the following will occur when hitting the ground.
        if (_pounding)
        {
            _animator.ResetTrigger("attack");
            velocity.y = _jumpPower / 1.4f;
            _animator.SetBool("pounded", true);
            GameManager.Instance.AudioSource.PlayOneShot(_poundSound);
            CameraEffects.Shake(200, 1f);
            _pounding = false;
            _recoveryCounter.Counter = 0;
            _animator.SetBool("pounded", true);
        }
    }

    public void FlashEffect()
    {
        //Flash the player quickly
        _animator.SetTrigger("flash");
    }

    public void Hide(bool hide)
    {
        Freeze(hide);

        foreach (SpriteRenderer sprite in _graphicSprites)
            sprite.gameObject.SetActive(!hide);
    }
}
