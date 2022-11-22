using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewPlayer : PhysicsObject
{
    [Header("Attributes")]
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float jumpPower = 10;
    public int attackPower = 25;
    [SerializeField] float attackDuration = .1f;
    bool frozen = false;
    bool isDying = false;
    public int health;
    int maxHealth = 100;
    [SerializeField] float fallForgiveness = 1; // amount which allow player to jump while falling
    [SerializeField] float fallForgivenessCounter; // counter which starts at the moment when the player falls

    [Header("Inventory")]
    [SerializeField] int coinsCollected;    

    [Header("References")]
    public AudioSource sfxAudioSource;
    public CameraEffects cameraEffects;
    public Dictionary<string, Sprite> inventory = new();    
    Vector2 healthBarOrigSize;
    Animator animator;
    AnimatorFunctions animatorFunctions;
    [SerializeField] AudioClip deathSound;
    [Range(0f, 1f)]
    [SerializeField] float deathSoundVolume;

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
        if (GameObject.Find("Original Player")) Destroy(gameObject);
        animator = GetComponent<Animator>();
        animatorFunctions = GetComponent<AnimatorFunctions>();
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.name = "Original Player";
        healthBarOrigSize = GameManager.Instance.healthBar.rectTransform.sizeDelta;
        SetHealth(health);
        SetSpawnLocation();
    }

    // Update is called once per frame
    void Update()
    {
        if (!frozen)
        {
            PlayerMovement();
            HandleAttack();
        }
        
        SetAnimatorValues();

        if (gameObject.transform.position.y < -15 && !isDying)
            StartCoroutine(Death());
    }

    private void SetAnimatorValues()
    {
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", velocity.y);
        animator.SetBool("grounded", grounded);
        animator.SetFloat("attackDirectionY", Input.GetAxis("Vertical"));
    }

    private void PlayerMovement()
    {
        targetVelocity = new Vector2(Input.GetAxis("Horizontal") * maxSpeed, 0);

        if (!grounded)
            fallForgivenessCounter += Time.deltaTime;
        else
            fallForgivenessCounter = 0;

        if (Input.GetButtonDown("Jump") && fallForgivenessCounter < fallForgiveness)
        {
            animatorFunctions.EmitParticles("step");
            velocity.y = jumpPower;
            grounded = false;
            fallForgivenessCounter = fallForgiveness;
        }

        if (targetVelocity.x < 0)
            transform.localScale = new Vector2(-1, 1);
        else if (targetVelocity.x > 0)
            transform.localScale = new Vector2(1, 1);
    }

    public void SetSpawnLocation()
    {
        transform.position = GameObject.Find("Spawn Location").transform.position;
    }

    public void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("attack");
        }
    }

    public void CoinCollected()
    {
        coinsCollected++;
        GameManager.Instance.coinsText.SetText(coinsCollected.ToString());
    }

    public void Hurt(int value)
    {
        StartCoroutine(FreezeEffect(.2f, .5f));
        animator.SetTrigger("hurt");
        cameraEffects.Shake(5, .5f);
        SetHealth(health - value);
    }

    public void AddHealth(int value)
    {
        SetHealth(health + value);
    }

    private void SetHealth(int value)
    {
        health = value;
        if (health > maxHealth) health = 100;

        if (health > 0 && health <= maxHealth)
        {
            GameManager.Instance.healthBar.rectTransform.sizeDelta = new Vector2(healthBarOrigSize.x * ((float)health / maxHealth), GameManager.Instance.healthBar.rectTransform.sizeDelta.y);
        }
        else if (health <= 0 && !isDying)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        isDying = true;
        frozen = true;
        sfxAudioSource.PlayOneShot(deathSound, deathSoundVolume);
        animator.SetBool("dead", true);
        animatorFunctions.EmitParticles("death");
        yield return new WaitForSeconds(2);
        ReloadLevel();
    }

    public IEnumerator FreezeEffect(float length, float timeScale)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSeconds(length);
        Time.timeScale = 1;
    }

    private void ReloadLevel()
    {
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Clear();
        }

        SetHealth(100);
        coinsCollected = 0;
        GameManager.Instance.coinsText.SetText(coinsCollected.ToString());
        RemoveInventoryItem(removeAll: true);
        SetSpawnLocation();
        frozen = false;
        animator.SetBool("dead", false);
        isDying = false;
        SceneManager.LoadScene("Level 1");        
    }

    public void AddInventoryItem(string inventoryName, Sprite image)
    {
        inventory.Add(inventoryName, image);
        GameManager.Instance.inventoryItemImage.sprite = inventory[inventoryName];
    }

    public void RemoveInventoryItem(string inventoryName = "", bool removeAll = false)
    {
        if (!string.IsNullOrEmpty(inventoryName))
            inventory.Remove(inventoryName);
        else if (removeAll)
            inventory.Clear();
        
        if (inventory.Count == 0)
            GameManager.Instance.inventoryItemImage.sprite = GameManager.Instance.inventoryItemBlank;
    }
}
