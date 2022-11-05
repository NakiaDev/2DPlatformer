using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    enum ItemType { Coin, Health, InventoryItem }
    public enum InventoryItemName { Key, KeyGem }

    [SerializeField] ItemType itemType;
    [SerializeField] InventoryItemName inventoryItemName;
    [SerializeField] Sprite inventoryItemSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            switch (itemType)
            {
                case ItemType.Coin:
                    NewPlayer.Instance.CoinCollected();
                    break;
                case ItemType.Health:
                    NewPlayer.Instance.ChangeHealthValue(10);
                    break;
                case ItemType.InventoryItem:
                    NewPlayer.Instance.AddInventoryItem(inventoryItemName.ToString(), inventoryItemSprite);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
