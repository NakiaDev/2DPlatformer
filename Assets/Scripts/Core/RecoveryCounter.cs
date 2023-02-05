using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryCounter : MonoBehaviour
{
    //This script can be attached to any gameObject that has an EnemyBase or Breakable script attached to it.
    //It ensures the EnemyBase or Breakable must recover by a certain length of time before the player can attack it again.

    [SerializeField] float _recoveryTime = 1f;
    [System.NonSerialized] public float Counter;
    [System.NonSerialized] public bool Recovering = false;

    // Update is called once per frame
    void Update()
    {
        if (Counter <= _recoveryTime)
        {
            Counter += Time.deltaTime;
            Recovering = true;
        }
        else
            Recovering = false;
    }
}
