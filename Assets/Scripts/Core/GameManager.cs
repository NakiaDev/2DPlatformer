using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioSource _audioSource; //A primary audioSource a large portion of game sounds are passed through
    [SerializeField] HUD _hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    Dictionary<string, Sprite> _inventory = new Dictionary<string, Sprite>();
    [SerializeField] AudioTrigger _gameMusic;
    [SerializeField] AudioTrigger _gameAmbience;

    // Properties
    public Dictionary<string, Sprite> Inventory { get { return _inventory; } }
    public HUD Hud { get { return _hud; } }
    public AudioSource AudioSource { get { return _audioSource; } }
    public AudioTrigger GameMusic { get { return _gameMusic; } }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    public void GetInventoryItem(string name, Sprite image)
    {
        _inventory.Add(name, image);

        if (image != null)
        {
            _hud.SetInventoryImage(_inventory[name]);
        }
    }

    public void RemoveInventoryItem(string name)
    {
        _inventory.Remove(name);
        _hud.SetInventoryImage(_hud.BlankUI);
    }

    public void ClearInventory()
    {
        _inventory.Clear();
        _hud.SetInventoryImage(_hud.BlankUI);
    }
}
