using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script can be placed on any collider that is a trigger. It can hurt enemies or the player, 
so we use it for both player attacks and enemy attacks. 
*/

public class AttackHit : MonoBehaviour
{
    enum AttacksWhat { EnemyBase, NewPlayer };
    [SerializeField] AttacksWhat _attacksWhat;
    [SerializeField] bool _oneHitKill;
    [SerializeField] float _startCollisionDelay; //Some enemy types, like EnemyBombs, should not be able blow up until a set amount of time
    int _targetSide = 1; //Is the attack target on the left or right side of this object?
    [SerializeField] GameObject _parent; //This must be specified manually, as some objects will have a parent that is several layers higher
    [SerializeField] int _hitPower = 1;

    void OnTriggerStay2D(Collider2D col)
    {
        //Determine which side the attack is on
        if (_parent.transform.position.x < col.transform.position.x)
            _targetSide = 1;
        else
            _targetSide = -1;

        //Determine what components we're hitting

        //Attack Player
        if (_attacksWhat == AttacksWhat.NewPlayer && col.GetComponent<NewPlayer>() != null)
            NewPlayer.Instance.GetHurt(_targetSide, _hitPower);
        //Attack Enemies
        else if (_attacksWhat == AttacksWhat.EnemyBase && col.GetComponent<EnemyBase>() != null)
            col.GetComponent<EnemyBase>().GetHurt(_targetSide, _hitPower);
        //Attack Breakables
        else if (_attacksWhat == AttacksWhat.EnemyBase && col.GetComponent<EnemyBase>() == null && col.GetComponent<Breakable>() != null)
            col.GetComponent<Breakable>().GetHurt(_hitPower);
    }
}
