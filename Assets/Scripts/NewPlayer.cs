using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayer : PhysicsObject
{
    [SerializeField] float maxSpeed = 1;
    [SerializeField] float jumpPower = 10;
    [SerializeField] int coinsCollected;
    [SerializeField] int health;
    int maxHealth = 100;
    Vector2 healthBarOrigSize;

    [Space(10)]
    public TextMeshProUGUI coinsText;
    public Image healthBar;
    public Dictionary<string, Sprite> inventory = new();
    public Image inventoryItemImage;
    public Sprite inventoryItemBlank;

    // Start is called before the first frame update
    void Start()
    {
        healthBarOrigSize = healthBar.rectTransform.sizeDelta;
        SetHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = new Vector2(Input.GetAxis("Horizontal") * maxSpeed, 0);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpPower;
        }
    }

    public void CoinCollected()
    {
        coinsCollected++;
        coinsText.SetText(coinsCollected.ToString());
    }

    public void AddHealthValue(int value)
    {
        SetHealth(health + value);
    }

    private void SetHealth(int value)
    {
        health = value;
        if (health > maxHealth) health = 100;
        
        if (health > 0 && health <= maxHealth)
        {
            healthBar.rectTransform.sizeDelta = new Vector2(healthBarOrigSize.x * ((float)health / maxHealth), healthBar.rectTransform.sizeDelta.y);
        }
        else if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void AddInventoryItem(string inventoryName, Sprite image)
    {
        inventory.Add(inventoryName, image);
        inventoryItemImage.sprite = inventory[inventoryName];
    }

    public void RemoveInventoryItem(string inventoryName)
    {
        inventory.Remove(inventoryName);
        
        if (inventory.Count == 0)
            inventoryItemImage.sprite = inventoryItemBlank;
    }
}
