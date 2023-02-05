using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used for coins, health, inventory items, and even ammo if you want to create a gun shooting mechanic!*/

public class Collectable : MonoBehaviour
{
    public enum ItemType { InventoryItem, Coin, Health }; //Creates an ItemType category
    [SerializeField] ItemType _itemType; //Allows us to select what type of item the gameObject is in the inspector
    [SerializeField] AudioClip _bounceSound;
    [SerializeField] AudioClip[] _collectSounds;
    [SerializeField] int _itemAmount;
    [SerializeField] string _itemName; //If an inventory item, what is its name?
    [SerializeField] Sprite _UIImage; //What image will be displayed if we collect an inventory item?

    void OnTriggerEnter2D(Collider2D col)
    {
        // layer 14: Collect me if I trigger with an object tagged "Death Zone", aka an area the player can fall to certain death
        if (col.gameObject == NewPlayer.Instance.gameObject || col.gameObject.layer == 14)
            Collect();
    }

    public void Collect()
    {
        switch (_itemType)
        {
            case ItemType.InventoryItem:
                if (_itemName != "")
                    GameManager.Instance.GetInventoryItem(_itemName, _UIImage);
                break;
            case ItemType.Coin:
                NewPlayer.Instance.Coins += _itemAmount;
                break;
            case ItemType.Health:
                if (NewPlayer.Instance.Health < NewPlayer.Instance.MaxHealth)
                {
                    GameManager.Instance.Hud.HealthBarHurt();
                    NewPlayer.Instance.Health += _itemAmount;
                }
                break;
        }

        GameManager.Instance.AudioSource.PlayOneShot(_collectSounds[Random.Range(0, _collectSounds.Length)], Random.Range(.6f, 1f));
        NewPlayer.Instance.FlashEffect();

        //If my parent has an Ejector script, it means that my parent is actually what needs to be destroyed, along with me, once collected
        if (transform.parent.GetComponent<Ejector>() != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);
    }
}
