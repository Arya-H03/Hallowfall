using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerSwordAttackState : PlayerBaseState
{
    [SerializeField] private float moveSpeedWhileAttaking = 2;
    [SerializeField] private float hitStopDuration = 0.05f;

    private float attackComboWindow = 0.6f;
    private float attackDelay = 0.4f;
    private bool canAttack = true;
    private bool isInCombo = false;
    private int comboIndex = 0;
    private Coroutine comboResetCoroutine;
    private AudioSource audioSource;

    [SerializeField] AudioClip[] attackSwingSFX;

    public delegate void EventHandler();
    public EventHandler OnFirstSwordSwingEvent;
    public EventHandler OnSecondSwordSwingEvent;
    public EventHandler OnThirdSwordSwingEvent;

    private Coroutine SpawnAfterImageCoroutine;

    private bool canDashAttack = false;

    [SerializeField] GameObject firstSwingEffect;
    [SerializeField] GameObject secondSwingEffect;
    [SerializeField] GameObject thirdSwingEffect;

    [SerializeField] int firstSwingDamage = 10;
    [SerializeField] int secondSwingDamage = 20;
    [SerializeField] int thirdSwingDamage = 30;

 
    [SerializeField] LayerMask layerMask; // Layer mask for the boxcast
    [SerializeField] float distance; // Distance for the boxcast in 2D

    [SerializeField] Transform firstSwingCenter;
    [SerializeField] private Vector2 firstSwingCastSize = new Vector2(1.7f, 1.5f);

    [SerializeField] Transform secondSwingCenter;
    [SerializeField] private Vector2 secondSwingCastSize = new Vector2(1.3f, 1f);

    [SerializeField] Transform thirdSwingCenter;
    [SerializeField] private Vector2 thirdSwingCastSize = new Vector2(1.2f, 2f);

    public Transform FirstSwingCenter { get => firstSwingCenter; }
    public Transform SecondSwingCenter { get => secondSwingCenter; }
    public Transform ThirdSwingCenter { get => thirdSwingCenter; }

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        OnFirstSwordSwingEvent += FirstSwingBoxCast;
        OnSecondSwordSwingEvent += SecondSwingBoxCast;
        OnThirdSwordSwingEvent += ThirdSwingBoxCast;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileAttaking;
        playerController.IsAttacking = true;

    }

    public override void OnExitState()
    {
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;
        canDashAttack = false;

        if (SpawnAfterImageCoroutine != null)
        {
            StopCoroutine(SpawnAfterImageCoroutine);
            SpawnAfterImageCoroutine = null;
        }
        playerController.rb.velocity = Vector2.zero;
    }

    public override void HandleState()
    {
         
    }


    public void HandleAttack()
    {
        if (!canAttack) return;

        comboIndex++;
        if (comboIndex > 3) comboIndex = 1;

        PlaySwingAnimation(comboIndex);

        canAttack = false;
        isInCombo = true;

        StartCoroutine(AttackCooldown());

        // Reset combo if the player waits too long
        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);
        comboResetCoroutine = StartCoroutine(ResetComboAfterDelay());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(attackComboWindow);
        comboIndex = 0; // Reset combo
        isInCombo = false;
    }

    private void PlaySwingAnimation(int comboIndex)
    {
        switch (comboIndex)
        {
            case 1:
               
                playerController.AnimationController.SetTriggerForAnimations("FirstSwing");               
                break;
            case 2:
             
                playerController.AnimationController.SetTriggerForAnimations("SecondSwing");
                break;
            case 3:
                
                playerController.AnimationController.SetTriggerForAnimations("ThirdSwing");
                break;
                
        };
    }
    public void HandleFirstSwing()
    {
        playerController.IsAttacking = true;
        playerController.CanPlayerAttack = false;
        canDashAttack = true;
        playerController.AnimationController.SetTriggerForAnimations("Attack");   
    }

    public void HandleDoubleSwing()
    {
        playerController.AnimationController.SetTriggerForAnimations("DoubleSwing");
    }
    public void HandleJumpAttack()
    {
        playerController.CanPlayerAttack = false;
        playerController.AnimationController.SetTriggerForAnimations("JumpAttack");
       
    }
    public void EndAttack()
    {       
        if (playerController.PlayerMovementManager.currentInputDir.x != 0)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else 
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }

        if (SpawnAfterImageCoroutine != null)
        {
            StopCoroutine(SpawnAfterImageCoroutine);
            SpawnAfterImageCoroutine = null;
        }
    }

    public void DashAttack()
    {
        if (canDashAttack)
        {
            canDashAttack = false;
            playerController.AnimationController.SetTriggerForAnimations("DoubleSwing");
            SpawnAfterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
            playerController.rb.velocity += new Vector2(8 * playerController.PlayerMovementManager.CurrentDirection.x, 0);
        }

    }
 
    private void OnSwordAttackBlockedByEnemy(RaycastHit2D hit)
    {
        Vector2 launchVector;
        if (hit.point.x >= this.transform.position.x)
        {
            launchVector = new Vector2(1.5f, 0);
        }
        else
        {
            launchVector = new Vector2(-1.5f, 0);
        }
        //hit.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, parent);
        hit.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hit.point);
    }

    private void FirstSwingBoxCast()
    {
        RaycastHit2D hitResult = BoxCastForAttack(firstSwingCenter, firstSwingCastSize);
        AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, attackSwingSFX.Length)]);
        SpawnSlashEffect(firstSwingEffect, firstSwingCenter.position);
        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("Enemy"))
            {
                
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
              
                enemyController.OnEnemyHit(firstSwingDamage, hitResult.point, HitSfxType.sword);
                GameManager.Instance.StopTime(hitStopDuration);
                
            }
            
        }
    }

    private void SecondSwingBoxCast()
    {

        RaycastHit2D hitResult = BoxCastForAttack(secondSwingCenter, secondSwingCastSize);
        AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, attackSwingSFX.Length)]);
        SpawnSlashEffect(secondSwingEffect, secondSwingCenter.position);
        if (hitResult.collider != null)
        {        
             if (hitResult.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
               
                enemyController.OnEnemyHit(secondSwingDamage, hitResult.point,HitSfxType.sword);
                GameManager.Instance.StopTime(hitStopDuration);
            }
           
        }
    }

    
    
    public void ThirdSwingBoxCast()
    {

        RaycastHit2D hitResult = BoxCastForAttack(thirdSwingCenter, thirdSwingCastSize);
        AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, attackSwingSFX.Length)]);
        SpawnSlashEffect(thirdSwingEffect, thirdSwingCenter.position);
        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.OnEnemyHit(thirdSwingDamage, hitResult.point,HitSfxType.sword);
                GameManager.Instance.StopTime(hitStopDuration);
            }
        }
    }

    private RaycastHit2D BoxCastForAttack(Transform centerPoint, Vector2 boxSize)
    {
        Vector2 direction = transform.right;
        RaycastHit2D closestHit;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(centerPoint.position.x, centerPoint.position.y), boxSize, 0f, direction, distance, layerMask);
        if (hits.Length > 0)
        {
            closestHit = hits[0];

            foreach (RaycastHit2D hit in hits)
            {
                if (Vector2.Distance(hit.point, this.transform.position) < Vector2.Distance(closestHit.point, this.transform.position))
                {
                    closestHit = hit;
                }
            }
            return closestHit;
        }

        return new RaycastHit2D();

    }

    private void SpawnSlashEffect(GameObject effect, Vector3 position)
    {

        //GameObject obj = Instantiate(effect, position, Quaternion.identity);
        //Vector3 scale = parent.transform.localScale;

        //Vector2 launchVec = Vector2.zero;
        //if (scale.x == 1)
        //{
        //    launchVec = new Vector2(1, 0);
        //}
        //if (scale.x == -1)
        //{
        //    launchVec = new Vector2(-1, 0);
        //    obj.transform.localScale = new Vector3(-2, 2, 2);
        //}
        //obj.GetComponent<Rigidbody2D>().velocity = launchVec * 4;
        //Destroy(obj, 0.2f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(secondSwingCenter.position, secondSwingCastSize);
    }
}
