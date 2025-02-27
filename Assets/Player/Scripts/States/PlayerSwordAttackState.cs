using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerBaseState
{
    private float moveSpeedWhileAttaking =0;
    private float hitStopDuration = 0;

    private float dashAttackDelay = 0;
    private float dashModifier = 0;
    private float attackComboWindow = 0;
    private float attackDelay = 0;
    private bool canAttack = true;
    private bool isInCombo = false;
    private int comboIndex = 0;
    private Coroutine comboResetCoroutine;
    private AudioSource audioSource;

    private AudioClip[] attackSwingSFX;
    private AudioClip[] dashAttackSFX;

    public delegate void EventHandler();
    public EventHandler OnFirstSwordSwingEvent;
    public EventHandler OnSecondSwordSwingEvent;
    public EventHandler OnThirdSwordSwingEvent;

    private Coroutine SpawnAfterImageCoroutine;

    private GameObject firstSwingEffect;
    private GameObject secondSwingEffect;
    private GameObject thirdSwingEffect;

    private float firstSwingDamage = 0;
    private float secondSwingDamage = 0;
    private float thirdSwingDamage = 0;
    private float dashAttackDamage = 0;

    private DashAttackBox dashAttackBox;
 
    private LayerMask layerMask; // Layer mask for the boxcast
    private float distance; // Distance for the boxcast in 2D

    [SerializeField] private Transform firstSwingCenter;
    private Vector2 firstSwingCastSize = Vector2.zero;

    [SerializeField] private Transform secondSwingCenter;
    private Vector2 secondSwingCastSize = Vector2.zero;

    [SerializeField] private Transform thirdSwingCenter;
    private Vector2 thirdSwingCastSize = Vector2.zero;

    public Transform FirstSwingCenter { get => firstSwingCenter; }
    public Transform SecondSwingCenter { get => secondSwingCenter; }
    public Transform ThirdSwingCenter { get => thirdSwingCenter; }
    public float DashAttackDamage { get => dashAttackDamage; set => dashAttackDamage = value; }

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dashAttackBox = GetComponentInChildren<DashAttackBox>();
    }

    public override void Start()
    {
        base.Start();  
        
        OnFirstSwordSwingEvent += FirstSwingBoxCast;
        OnSecondSwordSwingEvent += SecondSwingBoxCast;
        OnThirdSwordSwingEvent += ThirdSwingBoxCast;
    }

    public override void InitState(PlayerConfig config)
    {
        moveSpeedWhileAttaking = config.moveSpeedWhileAttaking;
        hitStopDuration = config.hitStopDuration;
        attackComboWindow = config.attackComboWindow;
        attackDelay = config.attackDelay;
        dashAttackDelay = config.dashAttackDelay;
        attackSwingSFX = config.attackSwingSFX;
        dashAttackSFX = config.dashAttackSFX;
        firstSwingDamage = config.firstSwingDamage;
        secondSwingDamage = config.secondSwingDamage;
        thirdSwingDamage = config.thirdSwingDamage;
        DashAttackDamage = config.dashAttackDamage;
        layerMask = config.layerMask;
        distance = config.distance;
        firstSwingCastSize = config.firstSwingCastSize;
        secondSwingCastSize = config.secondSwingCastSize;
        thirdSwingCastSize = config.thirdSwingCastSize;
        dashModifier = config.dashModifier;

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
        playerController.PlayerMovementManager.TurnPlayer(playerController.PlayerMovementManager.currentInputDir);
    }

    public override void HandleState()
    {
         
    }


    public void HandleAttack()
    {
        if (!canAttack) return;

        comboIndex++;
        if (comboIndex > 3) comboIndex = 1;

        playerController.PlayerMovementManager.TurnPlayerWithMousePos();

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

        AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, attackSwingSFX.Length)], 1);
    }
    public void HandleFirstSwing()
    {
        playerController.IsAttacking = true;
        playerController.CanPlayerAttack = false;
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
        if (playerController.PlayerMovementManager.currentInputDir != Vector2.zero)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else 
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
    }

    public void DashAttack()
    {
        StartCoroutine(DashAttackCoroutine());
    }

    private IEnumerator DashAttackCoroutine()
    {
        if (playerController.CanDashAttack)
        {
            //Play Anim
            playerController.AnimationController.SetTriggerForAnimations("Dash");
            //Enable Collider
            dashAttackBox.EnableCollider();
            //SFX
            AudioManager.Instance.PlaySFX(audioSource, attackSwingSFX[Random.Range(0, dashAttackSFX.Length)], 1);
            //After Image
            SpawnAfterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
            //Velocity
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            Vector2 dir = ( mousePos - playerController.transform.position).normalized;
            playerController.PlayerMovementManager.TurnPlayer(dir);
            dir.y = Mathf.Clamp(dir.y, -0.5f, 0.5f);
            playerController.rb.velocity += dir * dashModifier;

            playerController.CanDashAttack = false;

            //Local CD
            yield return new WaitForSeconds(0.5f);
            //End After Image
            if (SpawnAfterImageCoroutine != null)
            {
                StopCoroutine(SpawnAfterImageCoroutine);
                SpawnAfterImageCoroutine = null;
            }
            //Rest Vel
            playerController.rb.velocity = Vector2.zero;
            playerController.PlayerMovementManager.TurnPlayer(playerController.PlayerMovementManager.currentInputDir);
            //Disable Collider
            dashAttackBox.DisableCollider();

            EndAttack();

            yield return new WaitForSeconds(dashAttackDelay);
          
            playerController.CanDashAttack = true;

        }
    }

    private void FirstSwingBoxCast()
    {
        RaycastHit2D [] hitResult = BoxCastForAttack(firstSwingCenter.position, firstSwingCastSize);
        
       
        if (hitResult != null)
        {
            foreach (RaycastHit2D hit in hitResult)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject enemy = hit.collider.gameObject;
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    enemyController.OnEnemyHit(firstSwingDamage, hit.point, HitSfxType.sword);
                    GameManager.Instance.StopTime(hitStopDuration);

                }
            }
           
            
        }
    }

    private void SecondSwingBoxCast()
    {

        RaycastHit2D []hitResult = BoxCastForAttack(secondSwingCenter.position, secondSwingCastSize);

        if (hitResult != null)
        {
            foreach (RaycastHit2D hit in hitResult)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject enemy = hit.collider.gameObject;
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    enemyController.OnEnemyHit(secondSwingDamage, hit.point, HitSfxType.sword);
                    GameManager.Instance.StopTime(hitStopDuration);

                }
            }


        }
    }

    
    
    public void ThirdSwingBoxCast()
    {

        RaycastHit2D[] hitResult = BoxCastForAttack(thirdSwingCenter.position, thirdSwingCastSize);
        if (hitResult != null)
        {
            foreach (RaycastHit2D hit in hitResult)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject enemy = hit.collider.gameObject;
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    enemyController.OnEnemyHit(thirdSwingDamage, hit.point, HitSfxType.sword);
                    GameManager.Instance.StopTime(hitStopDuration);

                }
            }


        }
    }

    private RaycastHit2D[] BoxCastForAttack(Vector2 centerPoint, Vector2 boxSize)
    {
        Vector2 direction = transform.right;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(centerPoint, boxSize, 0f, direction, distance, layerMask);
        if (hits.Length > 0)
        {
            return hits;
        }

        return null;

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

    private void DebugBoxCast(Vector2 centerPoint, Vector2 boxSize)
    {
        Vector2 direction = transform.right;
        Color castColor = Color.red;
        Color hitColor = Color.green;

        // Draw the BoxCast starting position
        Vector2 worldCenter = centerPoint;
        Debug.DrawRay(worldCenter, direction * distance, castColor, 0.1f); // Draw the cast direction

        // Draw the BoxCast outline
        Vector2 halfSize = boxSize / 2f;
        Vector2 topLeft = worldCenter + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = worldCenter + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = worldCenter + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = worldCenter + new Vector2(halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, castColor, 0.1f);
        Debug.DrawLine(topRight, bottomRight, castColor, 0.1f);
        Debug.DrawLine(bottomRight, bottomLeft, castColor, 0.1f);
        Debug.DrawLine(bottomLeft, topLeft, castColor, 0.1f);

        // Perform the BoxCastAll
        RaycastHit2D[] hits = Physics2D.BoxCastAll(worldCenter, boxSize, 0f, direction, distance, layerMask);
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log($"Hit: {hit.collider.name} at {hit.point}");
            Debug.DrawRay(hit.point, Vector2.up * 0.5f, hitColor, 0.1f); // Draw where it hit
        }
    }

    
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    Gizmos.DrawWireCube(firstSwingCenter.position, firstSwingCastSize); 
    //}



}
