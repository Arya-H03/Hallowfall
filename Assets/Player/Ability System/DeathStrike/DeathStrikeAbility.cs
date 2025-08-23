using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathStrikeAbility : MonoBehaviour, IAbility
{
    private PlayerController playerController;
    private int damageBucket = 0;

    private List<Coroutine> damageInstanceCoroutineList = new();
    public void Init()
    {
        
    }

    public void PassPlayerControllerRef(PlayerController controller)
    {
        this.playerController = controller;
    }

    public void Perform()
    {
        playerController.PlayerSignalHub.OnPlayerHealthChange += AddToDamageBucket;
        playerController.PlayerSignalHub.OnSwordAttackHitFrame += TryDeathStrikeLogic;    
    }

    private void OnDisable()
    {
        playerController.PlayerSignalHub.OnPlayerHealthChange -= AddToDamageBucket;
    }
    private void AddToDamageBucket(int maxHealth, int currentHealth, int incomingDamage)
    {
        damageInstanceCoroutineList.Add(StartCoroutine(DamageInstanceCoroutine(incomingDamage)));
    }

    private IEnumerator DamageInstanceCoroutine(int amount)
    {
        damageBucket += Mathf.Abs(amount);
        yield return new WaitForSeconds(1);
        damageBucket -= Mathf.Abs(amount);
    }

    private void TryDeathStrikeLogic()
    {
        if (playerController.EnemyDetector.AvailableEnemyTargets.Count > 0)
        {
            playerController.PlayerSignalHub.OnRestoreHealth?.Invoke((int)(damageBucket * 0.5f));
            foreach(Coroutine coroutine in damageInstanceCoroutineList)
            {
                StopCoroutine(coroutine);
            }
            damageInstanceCoroutineList.Clear();
            damageBucket = 0;
        }
    }
}
