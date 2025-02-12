using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem.XR;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;
    private GameObject player;
    private EnemyAI enemyAI;

    public float maxHealth = 100;
    public float currentHealth;
    private void Awake()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(target.position);
    }

    public void OnEnemyDamage(float value)
    {
        if (currentHealth > 0)
        {
            currentHealth -= value;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                enemyAI.OnEnterDeathtState();

            }
        }
        else
        {
            //Debug.Log("You are dead");
        }
    }


}
