 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static UnityEngine.Rendering.DebugUI;

public class Player : MonoBehaviour
{
    public delegate void MyEventHandler();

    public static event MyEventHandler CorruptionEvent;

    public static event MyEventHandler LosingHealthShieldEvent;


    public static event MyEventHandler PlayerDeathEvent;
    public static event MyEventHandler PlayerRespawnEvent;

    private PlayerController controller;

    [SerializeField] PlayerHealthBar healthBar;

    [SerializeField] GameObject deathmenu;

    [SerializeField] Transform respawnTransform;
  
    public int maxCorupption = 100;
    public int currentCorupption = 0;

    public int numberOfHealthShield = 3;
    public int maxHealth = 100;
    public int currentHealth = 0;
    public bool isPlayerDead = false;
    private Material material;

    private SmartEnemyAgent agent;

    private int essenceCounter = 0;


    private void OnEnable()
    {
        PlayerDeathEvent += PlayerDeathCoroutine;
        PlayerRespawnEvent += PlayerRespawn;
        LosingHealthShieldEvent += OnHealthShieldLost;
    }

    private void OnDisable()
    {
        PlayerDeathEvent -= PlayerDeathCoroutine;
        PlayerRespawnEvent -= PlayerRespawn;
    }
    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        controller = GetComponent<PlayerController>();
        agent = FindAnyObjectByType<SmartEnemyAgent>();  
    }

    private void Start()
    {
        currentHealth = maxHealth;
        
    }

    public void OnGainCorruption(int value)
    {

        if (currentCorupption < maxCorupption)
        {
            currentCorupption += value;
            Debug.Log(currentCorupption);

            if (currentCorupption > maxCorupption)
            {
                currentCorupption = maxCorupption;
            }

            if(currentCorupption == maxCorupption)
            {
                ChangeShader();
            }
        }

        CorruptionEvent?.Invoke();
    }

    private IEnumerator FlashOnHit()
    {
        material.SetFloat("_Flash", 1);
        yield return new WaitForSeconds(0.5f);
        material.SetFloat("_Flash", 0);
    }
    public void OnTakingDamage(int value)
    {
        
        StartCoroutine(FlashOnHit());   
        if (currentHealth > 0)
        {
            
            controller.rb.velocity += controller.PlayerMovementManager.currentDirection * -5;
            currentHealth -= value;
            
            if (currentHealth <=0)
            {
                LosingHealthShieldEvent?.Invoke();

            }
        }
       
    }

    private void OnHealthShieldLost()
    {
        numberOfHealthShield--;
        if (numberOfHealthShield <= 0)
        {

            PlayerDeathEvent?.Invoke();
            //agent.SetReward(3f);
            //agent.EndEpisode();
        }
        currentHealth = maxHealth;
    }

    //public void OnKnockBack(Vector2 launchVector,float enemyXpos)
    //{
    //    if(this.transform.position.x - enemyXpos < 0)
    //    {
    //        controller.rb.velocity += new Vector2(- launchVector.x, launchVector.y);
    //    }
    //    else
    //    {
    //        controller.rb.velocity += new Vector2(launchVector.x, launchVector.y);
    //    }
        
    //}

    private void ChangeShader()
    {
        material.SetFloat("_isCorrupt", 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnGainCorruption(5);

        }

    }

    public void OnEssencePickUp()
    {
        essenceCounter++;
        Debug.Log(essenceCounter);
    }

    private void PlayerDeathCoroutine()
    {
        StartCoroutine(PlayerDeath());
    }

    private IEnumerator PlayerDeath()
    {
        controller.InputManager.OnDisable();
        controller.deathEffectParticle.Play();
        controller.spriteRenderer.enabled = false;
        //controller.PlayerMovementManager.StopRunning();
        yield return new WaitForSeconds(1f);
        controller.IsDead = true;
        isPlayerDead = true;
        //deathmenu.SetActive(true);
        OnPlayerRespawn();


    }

    public void OnPlayerDeath()
    {
        PlayerDeathEvent?.Invoke();
    }

    private void PlayerRespawn()
    {
        this.transform.position = respawnTransform.position;
        controller.InputManager.OnEnable();
        controller.spriteRenderer.enabled = true;
        controller.IsDead = false;
        isPlayerDead = false;
    }

    private void OnPlayerRespawn()
    {
        PlayerRespawnEvent?.Invoke();
    }
}
