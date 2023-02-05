using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [Header("Reference")]
    Animator _animator;
    [SerializeField] TextMeshProUGUI _coinsMesh;
    [SerializeField] GameObject _healthBar;
    [SerializeField] Image _inventoryItemGraphic;
    [SerializeField] GameObject _startUp;

    Sprite _blankUI; //The sprite that is shown in the UI when you don't have any items
    float _coins;
    float _coinsEased;
    float _healthBarWidth;
    float _healthBarWidthEased;
    public string LoadSceneName;

    // Properties
    public Sprite BlankUI { get { return _blankUI; } }
    public Animator Animator { get { return _animator; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        //Set all bar widths to 1, and also the smooth variables.
        _healthBarWidth = 1;
        _healthBarWidthEased = _healthBarWidth;
        _coins = NewPlayer.Instance.Coins;
        _coinsEased = _coins;
        _blankUI = _inventoryItemGraphic.GetComponent<Image>().sprite;
    }

    void Update()
    {
        //Update coins text mesh to reflect how many coins the player has! However, we want them to count up.
        _coinsMesh.text = Mathf.Round(_coinsEased).ToString();
        _coinsEased += (NewPlayer.Instance.Coins - _coinsEased) * Time.deltaTime * 5f;

        if (_coinsEased >= _coins)
        {
            _animator.SetTrigger("getGem");
            _coins = _coinsEased + 1;
        }

        //Controls the width of the health bar based on the player's total health
        _healthBarWidth = (float)NewPlayer.Instance.Health/ NewPlayer.Instance.MaxHealth;
        _healthBarWidthEased += (_healthBarWidth - _healthBarWidthEased) * Time.deltaTime * _healthBarWidthEased;
        _healthBar.transform.localScale = new Vector2(_healthBarWidthEased, 1);
    }

    public void HealthBarHurt()
    {
        _animator.SetTrigger("hurt");
    }

    public void SetInventoryImage(Sprite image)
    {
        _inventoryItemGraphic.sprite = image;
    }

    void ResetScene()
    {
        if (GameManager.Instance.Inventory.ContainsKey("reachedCheckpoint"))
        {
            //Send player back to the checkpoint if they reached one!
            NewPlayer.Instance.ResetLevel();
        }
        else
        {
            //Reload entire scene
            SceneManager.LoadScene(LoadSceneName);
        }
    }
}
