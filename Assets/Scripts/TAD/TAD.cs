//using System.Collections;
//using System.Collections.Generic;
//using Unity.MLAgents;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Audio;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

//public class TAD : MonoBehaviour
//{
//    public DialogueBox dialogueBox;

//    public GameObject enemy;

//    [SerializeField] Animator animator;

//    [SerializeField] Vector2 size;
//    [SerializeField] Transform loc;
//    [SerializeField] DamagePopUp damagePopUp;

//    [SerializeField] LayerMask layerMask;

//    public bool canAttack = true;
//    public bool isAttaking = false;

//    private float attackTimerCooldown = 3f;
//    public float attackTimer = 0f;
//    // Start is called before the first frame update

//    private float maxHealth = 100;
//    public float currentHealth;
    
//    void Start()
//    {
//        attackTimer = attackTimerCooldown;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        DrawCast();
//        if (attackTimer < attackTimerCooldown)
//        {
//            attackTimer += Time.deltaTime;
//            if (attackTimer >= attackTimerCooldown)
//            {
//                canAttack = true;
//            }
//        }

//        if (Vector2.Distance(enemy.transform.position, this.transform.position) <= 2 && canAttack)
//        {
//            Attack();
//        }

//        Vector2 dir = enemy.transform.position - this.transform.position;
//        TurnEnemy(dir);


//    }

//    private void Attack()
//    {
//        canAttack = false;       
//        animator.SetBool("isAttacking",true);
//        isAttaking = true;
//        attackTimer = 0;
//    }

//    public void EndAttack()
//    {
//        isAttaking = false;
//        animator.SetBool("isAttacking",false);
//    }
//    public void CastAttack()
//    {
//        //RaycastHit2D hit = Physics2D.BoxCast(new Vector2(loc.position.x, loc.position.y), size, 0f, transform.right, 0, layerMask);
//        RaycastHit2D hitResult = AttackBoxCast(loc, size);
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
//                hitResult.collider.gameObject.GetComponentInParent<BlockState>().OnAttackBlocked(30, launchVector, this.gameObject);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyCollisionManager>().SpawnImpactEffect(hitResult.point);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyController>().agent.SetReward(1f);


//            }

//            else if (hitResult.collider.CompareTag("Enemy"))
//            {
//                Vector2 launchVector = new Vector2(hitResult.point.x - this.transform.position.x, 10f);
//                GameObject enemy = hitResult.collider.gameObject;
//                EnemyController enemyController = enemy.GetComponent<EnemyController>();
//                enemy.GetComponent<EnemyCollisionManager>().OnEnemyHit(launchVector, 20);
//                hitResult.collider.gameObject.GetComponentInParent<EnemyController>().agent.SetReward(-1f);

//                if (!enemyController.hasSeenPlayer)
//                {
//                    enemyController.player = this.gameObject;
//                    enemyController.hasSeenPlayer = true;
//                    if (enemyController.CurrentStateEnum != EnemyStateEnum.Chase)
//                    {
//                        enemyController.ChangeState(EnemyStateEnum.Chase);
//                    }

//                }
//                enemyController.PlayBloodEffect(hitResult.point);
//            }
//        }
           

//        dialogueBox.StartDialouge(" Attack");
//    }

    
//    private RaycastHit2D AttackBoxCast(Transform centerPoint, Vector2 boxSize)
//    {
//        Vector2 direction = transform.right;
//        RaycastHit2D closestHit;

//        RaycastHit2D[] hits = Physics2D.BoxCastAll(new Vector2(centerPoint.position.x, centerPoint.position.y), boxSize, 0f, direction, 0, layerMask);
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

//    private void StartParry()
//    {
//        dialogueBox.StartDialouge(" Roll");
//    }



//    private void VisualizeBoxCast(Vector2 origin, Vector2 size, Vector2 direction, float distance)
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

//    public void DrawCast()
//    {
//        VisualizeBoxCast(loc.position, size, transform.right, 0);
//    }

//    private void TurnEnemy(Vector2 direction)
//    {

//        if (direction.x < 0)
//        {
//            transform.localScale = new Vector3(-1, 1, 1);

//        }
//        if (direction.x >= 0)
//        {
//            transform.localScale = new Vector3(1, 1, 1);
//        }
//    }

//    public void TakeDamage(int amount)
//    {
//        SpawnDamagePopUp(amount);
//        if (currentHealth > 0)
//        {
//            currentHealth -= amount;
//            if (currentHealth <= 0)
//            {
//                currentHealth = 0;
//                enemy.GetComponent<EnemyController>().agent.EndEpisode();
//                Debug.Log("TAD died");

//            }
//        }
//        else
//        {
//            enemy.GetComponent<EnemyController>().agent.EndEpisode();
//            //Debug.Log("You are dead");
//        }
//    }

//    public void ResetHealth()
//    {
//        currentHealth = maxHealth;
//    }

//    private void SpawnDamagePopUp(int damage)
//    {
//        DamagePopUp obj = Instantiate(damagePopUp, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
//        obj.SetText(damage.ToString());
//    }
//}
