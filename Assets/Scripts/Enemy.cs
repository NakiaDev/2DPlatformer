using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : PhysicsObject
{
    [Header("Attributes")]
    [SerializeField] float maxSpeed;
    [SerializeField] int attackPower = 10;
    [SerializeField] Vector2 rayCastOffset;
    [SerializeField] float rayCastLength = 2;
    [SerializeField] LayerMask rayCastLayerMask;
    [SerializeField] int direction = 1;
    int health = 100;
    int maxHealth = 100;

    [Header("References")]
    [SerializeField] ParticleSystem particleEnemyExplosion;
    RaycastHit2D rightLedgeRayCastHit;
    RaycastHit2D leftLedgeRayCastHit;
    RaycastHit2D rightWallRayCastHit;
    RaycastHit2D leftWallRayCastHit;

    Animator animator;

    [Header("Sound effects")]
    [SerializeField] AudioClip hurtSound;
    [Range(0f, 1f)]
    [SerializeField] float hurtSoundVolume = 1f;
    [SerializeField] AudioClip deathSound;
    [Range(0f, 1f)]
    [SerializeField] float deathSoundVolume = 1f;

    public int AttackPower { get => attackPower; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = new Vector2(maxSpeed * direction, 0);

        rightLedgeRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down, rayCastLength);
        Debug.DrawRay(new Vector2(transform.position.x + rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down * rayCastLength, Color.blue);
        if (rightLedgeRayCastHit.collider == null)
            direction = - 1;

        leftLedgeRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down, rayCastLength);
        Debug.DrawRay(new Vector2(transform.position.x - rayCastOffset.x, transform.position.y + rayCastOffset.y), Vector2.down * rayCastLength, Color.green);
        if (leftLedgeRayCastHit.collider == null)
            direction = 1;

        rightWallRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right, rayCastLength, rayCastLayerMask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.right * rayCastLength, Color.yellow);
        if (rightWallRayCastHit.collider != null)
            direction = -1;

        leftWallRayCastHit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left, rayCastLength, rayCastLayerMask);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + rayCastOffset.y), Vector2.left * rayCastLength, Color.cyan);
        if (leftWallRayCastHit.collider != null)
            direction = 1;
    }

    public void ChangeDirection()
    {
         direction *= -1;
    }

    public void Hurt(int value)
    {
        if (NewPlayer.Instance == null) return;

        health -= value;

        if (health <= 0)
        {
            NewPlayer.Instance.StartCoroutine(NewPlayer.Instance.FreezeEffect(.2f, .5f));

            if (deathSound != null)
                NewPlayer.Instance.sfxAudioSource.PlayOneShot(deathSound, deathSoundVolume);

            particleEnemyExplosion.transform.parent = gameObject.transform.parent;
            particleEnemyExplosion.gameObject.SetActive(true);
            NewPlayer.Instance.cameraEffects.Shake(5, .2f);
            
            Destroy(gameObject);
        }
        else 
        {
            animator.SetTrigger("hurt");

            if (hurtSound != null)
                NewPlayer.Instance.sfxAudioSource.PlayOneShot(hurtSound, hurtSoundVolume);
        }
    }
}
