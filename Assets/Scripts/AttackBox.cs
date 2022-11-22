using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    enum parentType { Player, Enemy}
    [SerializeField] parentType parent;
    [SerializeField] int targetSide;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collisionObjectToCheck = collision.gameObject;
        if (collisionObjectToCheck.transform.parent != null)
            collisionObjectToCheck = collisionObjectToCheck.transform.parent.gameObject;

        if (NewPlayer.Instance != null && parent == parentType.Player && collisionObjectToCheck.CompareTag("Enemy"))
        {
            collisionObjectToCheck.GetComponent<Enemy>().Hurt(NewPlayer.Instance.attackPower);
        }
        else if (parent == parentType.Enemy)
        {
            if (NewPlayer.Instance != null && collision.gameObject == NewPlayer.Instance.gameObject)
            {
                if (transform.parent.transform.position.x < collision.transform.position.x)
                    targetSide = -1;
                else
                    targetSide = 1;

                NewPlayer.Instance.Hurt(transform.parent.gameObject.GetComponent<Enemy>().AttackPower, targetSide);
            }
            else if (collisionObjectToCheck.CompareTag("Enemy"))
            {
                transform.parent.gameObject.GetComponent<Enemy>().ChangeDirection();
            }
        }
    }
}
