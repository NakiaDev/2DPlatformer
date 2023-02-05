using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] Collectable.ItemType requiredKeyName;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (NewPlayer.Instance != null && collision.gameObject == NewPlayer.Instance.gameObject)
        {
            if (GameManager.Instance.Inventory.ContainsKey(requiredKeyName.ToString()))
            {
                GameManager.Instance.RemoveInventoryItem(requiredKeyName.ToString());
                _animator.SetBool("opened", true);
            }
        }
    }
}
