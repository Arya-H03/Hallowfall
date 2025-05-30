using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    private GameObject target;
    private GameObject parryShield;
    [SerializeField] private bool canAttackBeParried = true;
    private bool isAttackParry = false;
    public GameObject Target { get => target; set => target = value; }
    public bool IsAttackParry { get => isAttackParry; set => isAttackParry = value; }
    public GameObject ParryShield { get => parryShield;}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canAttackBeParried && collision.CompareTag("ParryShield"))
        {
            parryShield = collision.gameObject;
            isAttackParry = true;
        }
        else if(collision.CompareTag("Player"))
        {
            target = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (canAttackBeParried && collision.CompareTag("ParryShield"))
        {
            parryShield = null;
            isAttackParry = false;
        }
        else if (collision.CompareTag("Player"))
        {
            target = null;
        }
    }
}
