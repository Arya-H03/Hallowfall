using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerBaseState
{
    public enum SwordAttackTypeEnum
    {
        Slash,
        Stab,
        Chop
    }

    private SwordAttackTypeEnum attackType;

    private AudioSource audioSource;
    [SerializeField] AudioClip[] hitClips;
    [SerializeField] AudioClip[] missClips;

    [SerializeField] PlayerFootSteps footSteps;
    private GameObject parent;

    //For Debuging
    //[SerializeField] Vector2 size; // Size of the box in 2D
    //[SerializeField] Transform loc; // Distance for the boxcast in 2D

    [SerializeField] GameObject slashEffect;
    [SerializeField] GameObject stabEffect;
    [SerializeField] GameObject chopEffect;

    [SerializeField] int slashDamage = 10;
    [SerializeField] int stabDamage = 20;
    [SerializeField] int chopDamage = 30;


    [SerializeField] LayerMask layerMask; // Layer mask for the boxcast
    [SerializeField] float distance; // Distance for the boxcast in 2D

    [SerializeField] Transform attack1BoxCastPosition;
    private Vector2 attack1BoxCastSize = new Vector2(1.5f, 1.5f);

    [SerializeField] Transform attack2BoxCastPosition;
    private Vector2 attack2BoxCastSize = new Vector2(1.75f, 0.35f);

    [SerializeField] Transform attack3BoxCastPosition;
    private Vector2 attack3BoxCastSize = new Vector2(1.75f, 0.35f);


    [SerializeField] private float moveSpeedWhileAttaking = 2;
    public SwordAttackTypeEnum AttackType { get => attackType; set => attackType = value; }

    public PlayerSwordAttackState()
    {
        this.stateEnum = PlayerStateEnum.SwordAttack;
    }
    public override void OnEnterState()
    {
        playerController.PlayerMovementManager.MoveSpeed = moveSpeedWhileAttaking;
        StartAttack(playerController.IsPlayerJumping, AttackType);
        
    }

    public override void OnExitState()
    {
        playerController.IsAttacking = false;
    }

    public override void HandleState()
    {


    }

    private void Awake()
    {
        parent = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }

    public void StartAttack(bool isJumping, SwordAttackTypeEnum attackType)
    {
        if (isJumping)
        {
            playerController.rb.gravityScale = 0.5f;
            playerController.rb.velocity = Vector2.zero;
            playerController.AnimationController.SetBoolForAnimations("isJumping",false);
            playerController.AnimationController.SetBoolForAnimations("isFalling", false);
            playerController.AnimationController.SetTriggerForAnimations("JumpAttack");
        }

        else
        {
            switch (attackType)
            {

                case SwordAttackTypeEnum.Slash:
                    playerController.AnimationController.SetTriggerForAnimations("Slash");

                    break;
                case SwordAttackTypeEnum.Stab:
                    playerController.AnimationController.SetTriggerForAnimations("Stab");
                    break;
                case SwordAttackTypeEnum.Chop:
                    playerController.AnimationController.SetTriggerForAnimations("Chop");
                    break;

            }
        }

        playerController.IsAttacking = true;

        

        

        

    }

    public void EndAttack()
    {
        playerController.rb.gravityScale = 3;
        playerController.IsAttacking = false;
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

    private void OnEnemyHit(RaycastHit2D hit,Vector2 launchVector,int damage)
    {
        GameObject enemy = hit.collider.gameObject;
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemy.GetComponent<EnemyCollisionManager>().OnEnemyHit(launchVector, damage);

        if (!enemyController.hasSeenPlayer)
        {
            enemyController.player = parent;
            enemyController.hasSeenPlayer = true;
            if (enemyController.currentStateEnum != EnemyStateEnum.Chase)
            {
                enemyController.ChangeState(EnemyStateEnum.Chase);
            }

        }
        audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
        enemyController.PlayBloodEffect(hit.point);
    }

    private Vector2 SlashLaunchVector(RaycastHit2D hit)
    {
        Vector2 launchVector = new Vector2(hit.point.x - this.transform.position.x, 10f);
        return launchVector;
    }

    private Vector2 StabLaunchVector(RaycastHit2D hit)
    {
        Vector2 launchVector;
        if (hit.point.x >= this.transform.position.x)
        {
            launchVector = new Vector2(4f, 0);
        }
        else
        {
            launchVector = new Vector2(-4f, 0);
        }
        return launchVector;
    }

    private Vector2 ChopLaunchVector(RaycastHit2D hit)
    {
        Vector2 launchVector = new Vector2(4f, 0);
        return launchVector;
    }
    public void SlashAttack()
    {
        RaycastHit2D hitResult = BoxCastForAttack(attack1BoxCastPosition, attack1BoxCastSize);
        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);

            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {
                OnEnemyHit(hitResult, SlashLaunchVector(hitResult), slashDamage);
            }


        }
        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }

        HandelSlashEffect(slashEffect, attack1BoxCastPosition.position);

    }

    public void StabAttack()
    {

        RaycastHit2D hitResult = BoxCastForAttack(attack2BoxCastPosition, attack2BoxCastSize);

        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);
            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {
                OnEnemyHit(hitResult, StabLaunchVector(hitResult), stabDamage);
            }
        }

        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }
        HandelSlashEffect(stabEffect, attack2BoxCastPosition.position + new Vector3(1, 0.35f, 0));
    }

    public void ChopAttack()
    {
        RaycastHit2D hitResult = BoxCastForAttack(attack3BoxCastPosition, attack3BoxCastSize);

        if (hitResult.collider != null)
        {
            if (hitResult.collider.CompareTag("EnemySwordBlock"))
            {
                OnSwordAttackBlockedByEnemy(hitResult);
            }

            else if (hitResult.collider.CompareTag("Enemy"))
            {

                OnEnemyHit(hitResult, ChopLaunchVector(hitResult), chopDamage);
            }
        }

        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }

        HandelSlashEffect(chopEffect, attack3BoxCastPosition.position);

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
    private void Update()
    {
        //VisualizeBoxCast(attack1BoxCastPosition.position, attack1BoxCastSize, transform.right, distance);
    }
}
