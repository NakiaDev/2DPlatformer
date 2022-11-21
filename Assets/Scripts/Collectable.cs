using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    enum ItemType { Coin, Health, InventoryItem }
    public enum InventoryItemName { Key, KeyGem }

    [Header("References")]
    [SerializeField] ItemType itemType;
    [SerializeField] InventoryItemName inventoryItemName;
    [SerializeField] Sprite inventoryItemSprite;
    [SerializeField] ParticleSystem particleCollectableSpark;

    [Header("Sound")]
    [SerializeField] AudioClip sound;
    [Range(0f, 1f)]
    [SerializeField] float soundVolume = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            if (particleCollectableSpark != null)
            {
                particleCollectableSpark.transform.parent = gameObject.transform.parent;
                particleCollectableSpark.gameObject.SetActive(true);
            }

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

            if (sound != null)
                NewPlayer.Instance.sfxAudioSource.PlayOneShot(sound, soundVolume * Random.Range(.8f, 1.4f));

            Destroy(gameObject);
        }
    }
}
