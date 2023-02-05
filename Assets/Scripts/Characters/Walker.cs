using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : PhysicsObject
{
    [Header("Reference")]
    EnemyBase _enemyBase;
    [SerializeField] GameObject _graphic;

    [Header("Properties")]
    [SerializeField] LayerMask _layerMask; //What can the Walker actually touch?
    enum EnemyType { Bug, Zombie }; //Bugs will simply patrol. Zombie's will immediately start chasing you forever until you defeat them.
    [SerializeField] EnemyType _enemyType;

    [SerializeField] float _attentionRange;
    [SerializeField] float _changeDirectionEase = 1; //How slowly should we change directions? A higher number is slower!
    [System.NonSerialized] public float Direction = 1;
    Vector2 _distanceFromPlayer; //How far is this enemy from the player?
    [System.NonSerialized] public float DirectionSmooth = 1; //The float value that lerps to the direction integer.
    [SerializeField] bool _followPlayer;
    [SerializeField] float _hurtLaunchPower = 10; //How much force should be applied to the player when getting hurt?
    [SerializeField] bool _jumping;
    [SerializeField] float _jumpPower = 7;
    [SerializeField] bool _jump = false;
    [System.NonSerialized] public float Launch = 1; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    [SerializeField] float _maxSpeed = 7;
    [SerializeField] float _maxSpeedDeviation; //How much should we randomly deviate from maxSpeed? Ensures enemies don't move at exact same speed, thus syncing up.
    [SerializeField] bool _neverStopFollowing = false; //Once the player is seen by an enemy, it will forever follow the player.
    Vector3 _origScale;
    [SerializeField] Vector2 _rayCastSize = new Vector2(1.5f, 1); //The raycast size: (Width, height)
    Vector2 _rayCastSizeOrig;
    [SerializeField] Vector2 _rayCastOffset;
    RaycastHit2D _rightWall;
    RaycastHit2D _leftWall;
    RaycastHit2D _rightLedge;
    RaycastHit2D _leftLedge;
    float _sitStillMultiplier = 1; //If 1, the enemy will move normally. But, if it is set to 0, the enemy will stop completely. 

    [SerializeField] bool _sitStillWhenNotFollowing = false; //Controls the sitStillMultiplier

    [Header("Sounds")]
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _stepSound;

    // Properties
    public float HurtLaunchPower { get { return _hurtLaunchPower; } }

    private void Awake()
    {
        _enemyBase = GetComponent<EnemyBase>();
    }

    private void Start()
    {
        _origScale = transform.localScale;
        _rayCastSizeOrig = _rayCastSize;
        _maxSpeed -= Random.Range(0, _maxSpeedDeviation);
        Launch = 0;

        if (_enemyType == EnemyType.Zombie)
        {
            Direction = 0;
            DirectionSmooth = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attentionRange);
    }

    protected override void ComputeVelocity()
    {
        base.ComputeVelocity();
        Vector2 move = Vector2.zero;
        if (!NewPlayer.Instance.Frozen)
        {
            _distanceFromPlayer = new Vector2(NewPlayer.Instance.gameObject.transform.position.x - transform.position.x, NewPlayer.Instance.gameObject.transform.position.y - transform.position.y);
            DirectionSmooth += ((Direction * _sitStillMultiplier) - DirectionSmooth) * Time.deltaTime * _changeDirectionEase;
            move.x = (1 * DirectionSmooth) + Launch;
            Launch += (0 - Launch) * Time.deltaTime;

            if (move.x < 0)
                transform.localScale = new Vector3(-_origScale.x, _origScale.y, _origScale.z);
            else
                transform.localScale = new Vector3(_origScale.x, _origScale.y, _origScale.z);

            if (!_enemyBase.RecoveryCounter.Recovering)
            {
                //Flip the graphic depending on the speed
                if (move.x > 0.01f)
                {
                    if (_graphic.transform.localScale.x == -1)
                        _graphic.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else if (move.x < -0.01f)
                {
                    if (_graphic.transform.localScale.x == 1)
                        _graphic.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }

                //Check if player is within range to follow
                if (_enemyType == EnemyType.Zombie)
                {
                    if ((Mathf.Abs(_distanceFromPlayer.x) < _attentionRange) && (Mathf.Abs(_distanceFromPlayer.y) < _attentionRange))
                    {
                        _followPlayer = true;
                        _sitStillMultiplier = 1;

                        if (_neverStopFollowing)
                            _attentionRange = 10000000000;
                    }
                    else
                        _sitStillMultiplier = _sitStillWhenNotFollowing ? 0 : 1;
                }

                if (_followPlayer)
                {
                    _rayCastSize.y = 200;
                    Direction = _distanceFromPlayer.x < 0 ? -1 : 1;
                }
                else
                    _rayCastSize.y = _rayCastSizeOrig.y;

                //Check for walls
                _rightWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + _rayCastOffset.y), Vector2.right, _rayCastSize.x, _layerMask);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + _rayCastOffset.y), Vector2.right * _rayCastSize.x, Color.yellow);

                if (_rightWall.collider != null)
                {
                    if (!_followPlayer)
                    {
                        Direction = -1;
                    }
                    else if (Direction == 1)
                    {
                        JumpA();
                    }

                }

                _leftWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + _rayCastOffset.y), Vector2.left, _rayCastSize.x, _layerMask);
                Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + _rayCastOffset.y), Vector2.left * _rayCastSize.x, Color.blue);

                if (_leftWall.collider != null)
                {
                    if (!_followPlayer)
                    {
                        Direction = 1;
                    }
                    else if (Direction == -1)
                    {
                        JumpA();
                    }
                }

                //Check for ledges. Walker's height check is much higher! They will fall pretty far, but will not fall to death. 
                _rightLedge = Physics2D.Raycast(new Vector2(transform.position.x + _rayCastOffset.x, transform.position.y), Vector2.down, _rayCastSize.y, _layerMask);
                Debug.DrawRay(new Vector2(transform.position.x + _rayCastOffset.x, transform.position.y), Vector2.down * _rayCastSize.y, Color.blue);
                if ((_rightLedge.collider == null || _rightLedge.collider.gameObject.layer == 14) && Direction == 1)
                {
                    Direction = -1;
                }

                _leftLedge = Physics2D.Raycast(new Vector2(transform.position.x - _rayCastOffset.x, transform.position.y), Vector2.down, _rayCastSize.y, _layerMask);
                Debug.DrawRay(new Vector2(transform.position.x - _rayCastOffset.x, transform.position.y), Vector2.down * _rayCastSize.y, Color.blue);
                if ((_leftLedge.collider == null || _leftLedge.collider.gameObject.layer == 14) && Direction == -1)
                {
                    Direction = 1;
                }
            }
        }

        _enemyBase.Animator.SetBool("grounded", grounded);
        _enemyBase.Animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / _maxSpeed);
        targetVelocity = move * _maxSpeed;
    }

    public void JumpA()
    {
        if (grounded)
        {
            velocity.y = _jumpPower;
            PlayJumpSound();
            PlayStepSound();
        }
    }

    public void PlayStepSound()
    {
        _enemyBase.AudioSource.pitch = (Random.Range(0.6f, 1f));
        _enemyBase.AudioSource.PlayOneShot(_stepSound);
    }

    public void PlayJumpSound()
    {
        _enemyBase.AudioSource.pitch = (Random.Range(0.6f, 1f));
        _enemyBase.AudioSource.PlayOneShot(_jumpSound);
    }
}
