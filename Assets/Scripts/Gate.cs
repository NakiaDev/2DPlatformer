using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] Collectable.InventoryItemName requiredKeyName;

    bool openGate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (openGate)
        {
            if (gameObject.transform.position.y >= 7.2f)
                openGate = false;
            else
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (3 * Time.deltaTime), gameObject.transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            NewPlayer player = collision.gameObject.GetComponent<NewPlayer>();

            if (player.inventory.ContainsKey(requiredKeyName.ToString()))
            {
                player.RemoveInventoryItem(requiredKeyName.ToString());
                openGate = true;
            }
        }
    }
}
