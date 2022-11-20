using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    enum parentType { Player, Enemy}
    [SerializeField] parentType parent;


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
        GameObject collisionObjectToCheck = collision.gameObject;
        if (collisionObjectToCheck.transform.parent != null)
            collisionObjectToCheck = collisionObjectToCheck.transform.parent.gameObject;

        if (parent == parentType.Player && collisionObjectToCheck.CompareTag("Enemy"))
        {
            collisionObjectToCheck.GetComponent<Enemy>().Hurt(NewPlayer.Instance.attackPower);
        }
        else if (parent == parentType.Enemy)
        {
            if (NewPlayer.Instance != null && collisionObjectToCheck == NewPlayer.Instance.gameObject)
            {
                NewPlayer.Instance.ChangeHealthValue(-transform.parent.gameObject.GetComponent<Enemy>().AttackPower);
            }
            else if (collisionObjectToCheck.CompareTag("Enemy"))
            {
                transform.parent.gameObject.GetComponent<Enemy>().ChangeDirection();
            }
        }
    }
}
