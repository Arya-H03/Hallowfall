using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyAttackZone : MonoBehaviour,IInitializeable<EnemyMeleeStrikeData>
{
    private GameObject target;
    private GameObject parryShield;
    private EnemyController owner;

    private float strikeDamage;
    private float parryDamage;

    [SerializeField] private bool canAttackBeParried = true;
  

    public void Init(EnemyMeleeStrikeData data)
    {
        owner = data.owner;   
        strikeDamage = data.strikeDamage;
        parryDamage = data.parryDamage;
    }

    private void Start()
    {
        Destroy(this.gameObject,1.5f);
    }
    public void TryHitTarget(EnemyController owner)
    {
        if(parryShield)
        {
            owner.EnemyPhysicsHandler.OnEnemyParried(parryDamage);
        }
        else if(target)
        {
            target.GetComponent<PlayerController>().PlayerSignalHub.OnPlayerHit?.Invoke(strikeDamage);
        }

        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canAttackBeParried && collision.CompareTag("ParryShield"))
        {
            parryShield = collision.gameObject;

        }
        else if(collision.CompareTag("Player"))
        {
            target = collision.gameObject.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (canAttackBeParried && collision.CompareTag("ParryShield"))
        {
            parryShield = null;
        }
        else if (collision.CompareTag("Player"))
        {
            target = null;
        }
    }
   
}



