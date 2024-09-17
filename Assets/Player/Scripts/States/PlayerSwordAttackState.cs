using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerBaseState
{
    public enum SwordAttackTypeEnum
    {
        firstSwing,
        secondSwing,
        Chop
    }

    private SwordAttackTypeEnum attackType;

    private AudioSource audioSource;

    [SerializeField] AudioClip[] swingMissAC;
    [SerializeField] AudioClip[] swingHitAC;

    private GameObject parent;

    private bool canDoubleSwing = false;

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
     private Vector2 firstSwingCastSize = new Vector2(1.1f, 1.5f);

    [SerializeField] Transform secondSwingCenter;
     private Vector2 secondSwingCastSize = new Vector2(1.3f, 0.8f);

    [SerializeField] Transform jumpAttackSwingCenter;
     private Vector2 jumpAttackSwingCastSize = new Vector2(1.2f, 2f);


    [SerializeField] private float moveSpeedWhileAttaking = 2;
    public SwordAttackTypeEnum AttackType { get => attackType; set => attackType = value; }
    public bool CanDoubleSwing { get => canDoubleSwing; set => canDoubleSwing = value; }

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }

    private void Awake()
    {
        parent = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileAttaking;
        StartAttack(playerController.IsPlayerGrounded);
        
    }

    public override void OnExitState()
    {
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;
        
    }

    public override void HandleState()
    {


    }

   
    public void StartAttack(bool isGrounded)
    {
        if (isGrounded)
        {
            playerController.AnimationController.SetTriggerForAnimations("Attack");
            playerController.IsAttacking = true;
            playerController.CanPlayerAttack = false;

        }

        else
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
    }

    public void DoubleSwing()
    {
        if (CanDoubleSwing)
        {
            playerController.AnimationController.SetTriggerForAnimations("DoubleSwing");
            CanDoubleSwing = false;
        }

    }

    public void EndAttack()
    {
        //playerController.rb.gravityScale = 3;
        playerController.IsAttacking = false;
        playerController.CanPlayerAttack = true;

        if (playerController.PlayerMovementManager.currentDirection.x != 0)
        {
            playerController.ChangeState(PlayerStateEnum.Run);
        }
        else 
        {
            playerController.ChangeState(PlayerStateEnum.Idle);
        }
        
        
        //footSteps.OnStartPlayerFootstep();

       

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

    public void FirstSwingAttack()
    {
        RaycastHit2D hitResult = BoxCastForAttack(firstSwingCenter, firstSwingCastSize);
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
                enemyController.OnEnemyHit(firstSwingDamage, hitResult.point,this.transform.parent.parent.gameObject);
                
            }


        }
        else
        {
            AudioManager.Instance.PlaySFX(audioSource, swingMissAC[Random.Range(0, swingMissAC.Length)]);
            
        }

        //HandelSlashEffect(firstSwingEffect, firstSwingCenter.position);

    }

    public void SecondSwingAttack()
    {

        RaycastHit2D hitResult = BoxCastForAttack(secondSwingCenter, secondSwingCastSize);

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
                enemyController.OnEnemyHit(secondSwingDamage, hitResult.point,this.gameObject);
            }
        }

        else
        {
            AudioManager.Instance.PlaySFX(audioSource, swingMissAC[Random.Range(0, swingMissAC.Length)]);
        }
        //HandelSlashEffect(secondSwingEffect, secondSwingCenter.position + new Vector3(1, 0.35f, 0));
    }

    public void JumpAttack()
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
                enemyController.OnEnemyHit(jumpAttackDamage, hitResult.point, this.gameObject);
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
    //void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    //{
    //    // Define the corners of the box for visualization in 2D
    //    Vector2 topLeft = origin + (Vector2.left * size.x / 2) + (Vector2.up * size.y / 2);
    //    Vector2 topRight = origin + (Vector2.right * size.x / 2) + (Vector2.up * size.y / 2);
    //    Vector2 bottomLeft = origin + (Vector2.left * size.x / 2) + (Vector2.down * size.y / 2);
    //    Vector2 bottomRight = origin + (Vector2.right * size.x / 2) + (Vector2.down * size.y / 2);

    //    // Draw the edges of the box using Debug.DrawLine for visualization in 2D
    //    Debug.DrawLine(topLeft, topRight, Color.red);
    //    Debug.DrawLine(topRight, bottomRight, Color.red);
    //    Debug.DrawLine(bottomRight, bottomLeft, Color.red);
    //    Debug.DrawLine(bottomLeft, topLeft, Color.red);

    //    // Draw the ray from the center to the right (assuming right is forward) for visualization in 2D
    //    Debug.DrawRay(origin, direction * distance, Color.red);
    //}
    //private void Update()
    //{
    //    VisualizeBoxCast(jumpAttackSwingCenter.position, jumpAttackSwingCastSize, transform.right, distance);
    //}
}
