//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerAttacks : MonoBehaviour
//{
//    private PlayerController playerController;
//    private AudioSource audioSource;

//    [SerializeField] AudioClip[] hitClips;
//    [SerializeField] AudioClip[] swingMissAC;

//    [SerializeField] PlayerFootSteps footSteps;
//    [SerializeField] Vector2 size; // Size of the box in 2D
//    [SerializeField] float distance; // Distance for the boxcast in 2D
//    [SerializeField] Transform loc; // Distance for the boxcast in 2D

//    [SerializeField] GameObject slash1;
//    [SerializeField] GameObject slash2;
//    [SerializeField] GameObject slash3;

//    [SerializeField] int attack1Damage = 10;
//    [SerializeField] int attack2Damage = 20;
//    [SerializeField] int attack3Damage = 30;

//    private GameObject parent;

//    public LayerMask layerMask; // Layer mask for the boxcast

//    [SerializeField] Transform attack1BoxCastPosition;
//    private Vector2 attack1BoxCastSize = new Vector2(1.5f, 1.5f);

//    [SerializeField] Transform attack2BoxCastPosition;
//    private Vector2 attack2BoxCastSize = new Vector2(1.75f, 0.35f);

//    [SerializeField] Transform attack3BoxCastPosition;
//    private Vector2 attack3BoxCastSize = new Vector2(1.75f, 0.35f);
//    //private Vector2 firstSwingCenter = new Vector2(1.056f, 0.025f);

//    private void Start()
//    {
//        playerController = GetComponentInParent<PlayerController>();
//        parent = GameObject.FindGameObjectWithTag("Player");
//        audioSource = GetComponent<AudioSource>();
//    }
//    public void Attack(bool isJumping, int attackIndex)
//    {
//        playerController.AnimationController.EndAnimations("");
//        switch (attackIndex)
//        {

//            case 1:
//                //Slash
//                playerController.AnimationController.SetTriggerForAnimations("FirstSwingAttack");
                
//                break;
//                //stab
//            case 2:
//                playerController.AnimationController.SetTriggerForAnimations("SecondSwingAttack");
//                break;
//                //Chop
//            case 3:
//                playerController.AnimationController.SetTriggerForAnimations("ChopAttack");
//                break;
                
//        }

//        playerController.IsAttacking = true;

//        if (isJumping)
//        {
//            playerController.rb.gravityScale = 0.2f;
//            playerController.rb.velocity = Vector2.zero;
//        }

//    }

//    public void EndAttack()
//    {
//        playerController.rb.gravityScale = 3;
//        if (playerController.PlayerMovementManager.currentDirection.x != 0)
//        {
//            playerController.AnimationController.SetBoolForAnimations("isRunning", true);
//        }
//        playerController.IsAttacking = false;
//        footSteps.OnStartPlayerFootstep();

//    }
//    public void Attack1()
//    {
//        RaycastHit2D hitResult = AttackBoxCast(attack1BoxCastPosition, attack1BoxCastSize);
//        if (hitResult.collider != null)
//        {
//            if (hitResult.collider.CompareTag("EnemySwordBlock"))
//            {
//                Vector2 launchVector;
//                if (hitResult.point.x >= this.transform.position.x)
//                {
//                    launchVector = new Vector2(1.5f, 0);
//                }
//                else
//                {
//                    launchVector = new Vector2(-1.5f, 0);
//                }
//                hitResult.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, parent);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hitResult.point);

//            }

//            else if (hitResult.collider.CompareTag("Enemy"))
//            {
//                Vector2 launchVector = new Vector2(hitResult.point.x - this.transform.position.x, 10f);
//                GameObject enemy = hitResult.collider.gameObject;
//                EnemyController enemyController = enemy.GetComponent<EnemyController>();
//                enemy.GetComponent<EnemyCollisionManager>().OnEnemyHit(launchVector, attack1Damage);

//                if (!enemyController.hasSeenPlayer)
//                {
//                    enemyController.player = parent;
//                    enemyController.hasSeenPlayer = true;
//                    if (enemyController.CurrentStateEnum != EnemyStateEnum.Chase)
//                    {
//                        enemyController.ChangeState(EnemyStateEnum.Chase);
//                    }

//                }
//                audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
//                enemyController.PlayBloodEffect(hitResult.point);
//            }


//        }
//        else
//        {
//            audioSource.PlayOneShot(swingMissAC[Random.Range(0, 3)]);
//        }

//        HandelSlashEffect(slash1, attack1BoxCastPosition.position);

//    }

//    public void Attack2()
//    {

//        RaycastHit2D hitResult = AttackBoxCast(attack2BoxCastPosition, attack2BoxCastSize);

//        if (hitResult.collider != null)
//        {
//            if (hitResult.collider.CompareTag("EnemySwordBlock"))
//            {
//                Debug.Log("Block");
//                Vector2 launchVector;
//                if (hitResult.point.x >= this.transform.position.x)
//                {
//                    launchVector = new Vector2(1.5f, 0);
//                }
//                else
//                {
//                    launchVector = new Vector2(-1.5f, 0);
//                }
//                //hitResult.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().OnEnemyHit(launchVector, 0);
//                hitResult.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, parent);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hitResult.point);

//            }

//            else if (hitResult.collider.CompareTag("Enemy"))
//            {
//                Vector2 launchVector;
//                if (hitResult.point.x >= this.transform.position.x)
//                {
//                    launchVector = new Vector2(4f, 0);
//                }
//                else
//                {
//                    launchVector = new Vector2(-4f, 0);
//                }

//                GameObject enemy = hitResult.collider.gameObject;
//                EnemyController enemyController = enemy.GetComponent<EnemyController>();
//                enemy.GetComponent<EnemyCollisionManager>().OnEnemyHit(launchVector, attack2Damage);

//                if (!enemyController.hasSeenPlayer)
//                {
//                    enemyController.player = parent;
//                    enemyController.hasSeenPlayer = true;
//                    if (enemyController.CurrentStateEnum != EnemyStateEnum.Chase)
//                    {
//                        enemyController.ChangeState(EnemyStateEnum.Chase);
//                    }
//                }
//                audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
//                enemyController.PlayBloodEffect(hitResult.point);
//            }
//        }

//        else
//        {
//            audioSource.PlayOneShot(swingMissAC[Random.Range(0, 3)]);
//        }
//        HandelSlashEffect(slash2, attack2BoxCastPosition.position + new Vector3(1,0.35f,0));
//    }

//    public void Attack3()
//    {
//        RaycastHit2D hitResult = AttackBoxCast(attack3BoxCastPosition, attack3BoxCastSize);

//        if (hitResult.collider != null)
//        {
//            if (hitResult.collider.CompareTag("EnemySwordBlock"))
//            {
//                Debug.Log("Block");
//                Vector2 launchVector;
//                if (hitResult.point.x >= this.transform.position.x)
//                {
//                    launchVector = new Vector2(1.5f, 0);
//                }
//                else
//                {
//                    launchVector = new Vector2(-1.5f, 0);
//                }
//                hitResult.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, parent);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hitResult.point);

//            }

//            else if (hitResult.collider.CompareTag("Enemy"))
//            {

//                Vector2 launchVector = new Vector2(4f, 0);

//                GameObject enemy = hitResult.collider.gameObject;
//                EnemyController enemyController = enemy.GetComponent<EnemyController>();    
//                enemy.GetComponent<EnemyCollisionManager>().OnEnemyHit(launchVector, attack3Damage);

//                if (!enemyController.hasSeenPlayer)
//                {
//                    enemyController.player = parent;
//                    enemyController.hasSeenPlayer = true;
//                    if (enemyController.CurrentStateEnum != EnemyStateEnum.Chase)
//                    {
//                        enemyController.ChangeState(EnemyStateEnum.Chase);
//                    }
//                }
//                audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
//                enemyController.PlayBloodEffect(hitResult.point);
//            }

           
           
            
//        }

//        else
//        {
//            audioSource.PlayOneShot(swingMissAC[Random.Range(0, 3)]);
//        }

//        HandelSlashEffect(slash3, attack3BoxCastPosition.position);

//    }

//    private RaycastHit2D AttackBoxCast(Transform centerPoint, Vector2 boxSize)
//    {
//        Vector2 direction = transform.right;
//        RaycastHit2D closestHit;

//        RaycastHit2D []hits = Physics2D.BoxCastAll(new Vector2(centerPoint.position.x, centerPoint.position.y), boxSize, 0f, direction, distance, layerMask);
//        if (hits.Length > 0)
//        {
//            closestHit = hits[0];

//            foreach (RaycastHit2D hit in hits)
//            {
//                if (Vector2.Distance(hit.point, this.transform.position) < Vector2.Distance(closestHit.point, this.transform.position))
//                {
//                    closestHit = hit;
//                }
//            }
//            return closestHit;
//        }

//        return new RaycastHit2D();
        
//    }
//    void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
//    {
//        // Define the corners of the box for visualization in 2D
//        Vector2 topLeft = origin + (Vector2.left * size.x / 2) + (Vector2.up * size.y / 2);
//        Vector2 topRight = origin + (Vector2.right * size.x / 2) + (Vector2.up * size.y / 2);
//        Vector2 bottomLeft = origin + (Vector2.left * size.x / 2) + (Vector2.down * size.y / 2);
//        Vector2 bottomRight = origin + (Vector2.right * size.x / 2) + (Vector2.down * size.y / 2);

//        // Draw the edges of the box using Debug.DrawLine for visualization in 2D
//        Debug.DrawLine(topLeft, topRight, Color.red);
//        Debug.DrawLine(topRight, bottomRight, Color.red);
//        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
//        Debug.DrawLine(bottomLeft, topLeft, Color.red);

//        // Draw the ray from the center to the right (assuming right is forward) for visualization in 2D
//        Debug.DrawRay(origin, direction * distance, Color.red);
//    }

//    private void HandelSlashEffect(GameObject effect, Vector3 position)
//    {
        
//        GameObject obj = Instantiate(effect, position, Quaternion.identity);
//        Vector3 scale = parent.transform.localScale;
       
//        Vector2 launchVec = Vector2.zero;
//        if (scale.x == 1)
//        {
//            launchVec = new Vector2(1, 0);
//        }
//        if (scale.x == -1)
//        {
//            launchVec = new Vector2(-1, 0);
//            obj.transform.localScale = new Vector3(-2,2,2);
//        }
//        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 5;
//        Destroy(obj, 0.5f);
//    }

//    private void Update()
//    {
//        //VisualizeBoxCast(firstSwingCenter.position, firstSwingCastSize, transform.right, distance);
//    }
//}
