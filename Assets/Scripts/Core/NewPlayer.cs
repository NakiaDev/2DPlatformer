using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayer : PhysicsObject
{
    [Header("Attributes")]
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float jumpPower = 10;
    public int attackPower = 25;
    [SerializeField] float attackDuration = .1f;

    [Header("Inventory")]
    [SerializeField] int coinsCollected;
    [SerializeField] int health;
    int maxHealth = 100;

    //[Header("References")]
    public Dictionary<string, Sprite> inventory = new();    
    Vector2 healthBarOrigSize;
    Animator animator;


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
        PlayerMovement();
        HandleAttack();
        SetAnimatorValues();
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

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpPower;
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

    public void ChangeHealthValue(int value)
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
        else if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AddInventoryItem(string inventoryName, Sprite image)
    {
        inventory.Add(inventoryName, image);
        GameManager.Instance.inventoryItemImage.sprite = inventory[inventoryName];
    }

    public void RemoveInventoryItem(string inventoryName)
    {
        inventory.Remove(inventoryName);
        
        if (inventory.Count == 0)
            GameManager.Instance.inventoryItemImage.sprite = GameManager.Instance.inventoryItemBlank;
    }
}
