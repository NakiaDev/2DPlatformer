using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] Collectable.InventoryItemName requiredKeyName;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == NewPlayer.Instance.gameObject)
        {
            if (NewPlayer.Instance.inventory.ContainsKey(requiredKeyName.ToString()))
            {
                NewPlayer.Instance.RemoveInventoryItem(requiredKeyName.ToString());
                animator.SetBool("opened", true);
            }
        }
    }
}
