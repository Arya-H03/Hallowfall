using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerSwordAttackState : PlayerBaseState
{
  
    private AudioSource audioSource;

    [SerializeField] AudioClip[] swingMissAC;
    [SerializeField] AudioClip[] swingHitAC;

    private GameObject parent;

    public delegate void EventHandler();
    public EventHandler OnFirstSwordSwingEvent;
    public EventHandler OnSecondSwordSwingEvent;

    private Coroutine SpawnAfterImageCoroutine;

    private bool canDashAttack = false;

    //For Debuging
    [SerializeField] Vector2 size; // Size of the box in 2D
    [SerializeField] Transform loc; // Distance for the boxcast in 2D

    [SerializeField] GameObject firstSwingEffect;
    [SerializeField] GameObject secondSwingEffect;
    [SerializeField] GameObject chopEffect;

    [SerializeField] int firstSwingDamage = 10;
    [SerializeField] int secondSwingDamage = 20;
    [SerializeField] int jumpAttackDamage = 15;


    [SerializeField] LayerMask layerMask; // Layer mask for the boxcast
    [SerializeField] float distance; // Distance for the boxcast in 2D

    [SerializeField] Transform firstSwingCenter;
     private Vector2 firstSwingCastSize = new Vector2(1.7f, 1.5f);

    [SerializeField] Transform secondSwingCenter;
     private Vector2 secondSwingCastSize = new Vector2(1.3f, 1f);

    [SerializeField] Transform jumpAttackSwingCenter;
     private Vector2 jumpAttackSwingCastSize = new Vector2(1.2f, 2f);

    private Vector3 airStrikeTargetPos;
    private bool airStrikeHasTarget = false;


    [SerializeField] private float moveSpeedWhileAttaking = 2;
   
    public Transform FirstSwingCenter { get => firstSwingCenter; set => firstSwingCenter = value; }
    public Transform SecondSwingCenter { get => secondSwingCenter; set => secondSwingCenter = value; }

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    private void Awake()
    {
        parent = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        OnFirstSwordSwingEvent += FirstSwingBoxCast;
        OnSecondSwordSwingEvent += SecondSwingBoxCast;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileAttaking;
       
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
    }

    public override void HandleState()
    {
        if(airStrikeHasTarget && Vector3.Distance(playerController.transform.position, airStrikeTargetPos) < 2f)
        {
            EndAttack();
            airStrikeHasTarget = false;
        }
       
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
        playerController.IsAttacking = true;
        playerController.CanPlayerAttack = false;
        playerController.AnimationController.SetTriggerForAnimations("JumpAttack");
       
    }
    public void EndAttack()
    {       
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;
        canDashAttack = false;

        if (!playerController.IsPlayerGrounded)
        {
            playerController.ChangeState(PlayerStateEnum.Fall);
        }
        else if (playerController.PlayerMovementManager.currentInputDir.x != 0)
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
            playerController.AnimationController.SetTriggerForAnimations("DoubleSwing");
            SpawnAfterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
            playerController.rb.velocity += new Vector2(8 * playerController.PlayerMovementManager.CurrentDirection.x, 0);
        }

    }
    public void AirStrike()
    {
        //var targetPos = Input.mousePosition;
        //targetPos = Camera.main.ScreenToWorldPoint(targetPos);
        //targetPos.z = this.transform.position.z;
        //airStrikeTargetPos = targetPos;
        //airStrikeHasTarget = true;
        //Vector2 attackVec = (airStrikeTargetPos - playerController.transform.position).normalized;
        //playerController.PlayerMovementManager.TurnPlayer(attackVec);
        //SpawnAfterImageCoroutine = StartCoroutine(playerController.AfterImageHandler.SpawnImage());
        //playerController.PlayerCollision.Rb.velocity = attackVec * 5f;
        //playerController.IsAttacking = true;
        //playerController.CanPlayerAttack = false;

       // playerController.ChangeState(PlayerStateEnum.Fall);
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
        hit.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, parent);
        hit.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hit.point);
    }

    private void FirstSwingBoxCast()
    {
        RaycastHit2D hitResult = BoxCastForAttack(FirstSwingCenter, firstSwingCastSize);
        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);

            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {
                
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                AudioManager.Instance.PlaySFX(audioSource, swingHitAC[Random.Range(0, swingHitAC.Length)]);
                enemyController.OnEnemyHit(firstSwingDamage, hitResult.point/*this.transform.parent.parent.gameObject*/);
                
            }


        }
        else
        {
            AudioManager.Instance.PlaySFX(audioSource, swingMissAC[Random.Range(0, swingMissAC.Length)]);
            
        }

        //HandelSlashEffect(firstSwingEffect, firstSwingCenter.position);

    }

    private void SecondSwingBoxCast()
    {

        RaycastHit2D hitResult = BoxCastForAttack(SecondSwingCenter, secondSwingCastSize);
        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);
            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                AudioManager.Instance.PlaySFX(audioSource, swingHitAC[Random.Range(0, swingHitAC.Length)]);
                enemyController.OnEnemyHit(secondSwingDamage, hitResult.point/*,this.gameObject*/);
            }
        }

        else
        {
            AudioManager.Instance.PlaySFX(audioSource, swingMissAC[Random.Range(0, swingMissAC.Length)]);
        }
        //HandelSlashEffect(secondSwingEffect, secondSwingCenter.position + new Vector3(1, 0.35f, 0));
    }

    
    
    public void JumpAttackBoxCast()
    {

        RaycastHit2D hitResult = BoxCastForAttack(jumpAttackSwingCenter, jumpAttackSwingCastSize);

        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);
            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {
                GameObject enemy = hitResult.collider.gameObject;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                enemyController.OnEnemyHit(jumpAttackDamage, hitResult.point);
            }
        }

        else
        {
            audioSource.PlayOneShot(swingMissAC[Random.Range(0, 3)]);
        }
        //HandelSlashEffect(secondSwingEffect, secondSwingCenter.position + new Vector3(1, 0.35f, 0));
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

    private void HandelSlashEffect(GameObject effect, Vector3 position)
    {

        GameObject obj = Instantiate(effect, position, Quaternion.identity);
        Vector3 scale = parent.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-2, 2, 2);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 5;
        Destroy(obj, 0.5f);
    }

    //For debugging
    void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    {
        // Define the corners of the box for visualization in 2D
        Vector2 topLeft = origin + (Vector2.left * size.x / 2) + (Vector2.up * size.y / 2);
        Vector2 topRight = origin + (Vector2.right * size.x / 2) + (Vector2.up * size.y / 2);
        Vector2 bottomLeft = origin + (Vector2.left * size.x / 2) + (Vector2.down * size.y / 2);
        Vector2 bottomRight = origin + (Vector2.right * size.x / 2) + (Vector2.down * size.y / 2);

        // Draw the edges of the box using Debug.DrawLine for visualization in 2D
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);

        // Draw the ray from the center to the right (assuming right is forward) for visualization in 2D
        Debug.DrawRay(origin, direction * distance, Color.red);
    }
    //private void Update()
    //{
    //    //VisualizeBoxCast(FirstSwingCenter.position, firstSwingCastSize, transform.right, distance);
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(secondSwingCenter.position, secondSwingCastSize);
    //}
}
