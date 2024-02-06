using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    private PlayerController playerController;
    private AudioSource audioSource;

    [SerializeField] AudioClip[] hitClips;
    [SerializeField] AudioClip[] missClips;

    [SerializeField] PlayerFootSteps footSteps;
    [SerializeField] Vector2 size; // Size of the box in 2D
    [SerializeField] float distance; // Distance for the boxcast in 2D
    [SerializeField] Transform loc; // Distance for the boxcast in 2D

    [SerializeField] GameObject slash1;
    [SerializeField] GameObject slash2;
    [SerializeField] GameObject slash3;

    [SerializeField] int attack1Damage = 10;
    [SerializeField] int attack2Damage = 20;
    [SerializeField] int attack3Damage = 30;

    private GameObject parent;

    public LayerMask layerMask; // Layer mask for the boxcast

    [SerializeField] Transform attack1BoxCastPosition;
    private Vector2 attack1BoxCastSize = new Vector2(1.3f, 1.5f);

    [SerializeField] Transform attack2BoxCastPosition;
    private Vector2 attack2BoxCastSize = new Vector2(1.75f, 0.35f);

    [SerializeField] Transform attack3BoxCastPosition;
    private Vector2 attack3BoxCastSize = new Vector2(1.75f, 0.35f);
    //private Vector2 attack1BoxCastPosition = new Vector2(1.056f, 0.025f);

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        parent = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }
    public void StartAttack(bool isJumping, int attackIndex)
    {
        //playerController.animationController.SetBoolForAnimations("isRunning", false);
        //dplayerController.animationController.SetBoolForAnimations("isJumping", false);
        playerController.animationController.EndAnimations("");
        switch (attackIndex)
        {
            case 1:
                playerController.animationController.SetTriggerForAnimations("Attack1");
                
                break;
            case 2:
                playerController.animationController.SetTriggerForAnimations("Attack2");
                break;
            case 3:
                playerController.animationController.SetTriggerForAnimations("Attack3");
                break;
        }

        if (isJumping)
        {
            playerController.rb.gravityScale = 0.2f;
            playerController.rb.velocity = Vector2.zero;
        }

    }

    public void EndAttack()
    {
        playerController.rb.gravityScale = 3;
        if (playerController.playerMovementManager.currentDirection.x != 0)
        {
            playerController.animationController.SetBoolForAnimations("isRunning", true);
        }

        footSteps.OnStartPlayerFootstep();

    }
    public void Attack1BoxCast()
    {      
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(attack1BoxCastPosition.position.x, attack1BoxCastPosition.position.y), attack1BoxCastSize, 0f, direction, distance, layerMask);

        if (hit.collider != null)
        {
            Vector2 launchVector = new Vector2(hit.point.x - this.transform.position.x, 10f);
            GameObject enemy = hit.collider.gameObject;
            enemy.GetComponent<EnemyCollision>().OnEnemyHit(launchVector, attack1Damage);

            audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
        }
        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }

        HandelSlashEffect(slash1, attack1BoxCastPosition.position);

    }

    public void Attack2BoxCast()
    {

        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(attack2BoxCastPosition.position.x, attack2BoxCastPosition.position.y), attack2BoxCastSize, 0f, direction, distance, layerMask);

        if (hit.collider != null)
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

            GameObject enemy = hit.collider.gameObject;
            enemy.GetComponent<EnemyCollision>().OnEnemyHit(launchVector, attack2Damage);
            audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
        }

        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }
        HandelSlashEffect(slash2, attack2BoxCastPosition.position + new Vector3(1,0.35f,0));
    }

    public void Attack3BoxCast()
    {
        Vector2 direction = transform.right;

        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(attack3BoxCastPosition.position.x, attack3BoxCastPosition.position.y), attack3BoxCastSize, 0f, direction, distance, layerMask);

        if (hit.collider != null)
        {
            Vector2 launchVector = new Vector2(4f, 0);
            GameObject enemy = hit.collider.gameObject;
            enemy.GetComponent<EnemyCollision>().OnEnemyHit(launchVector, attack3Damage);
            audioSource.PlayOneShot(hitClips[Random.Range(0, 3)]);
        }

        else
        {
            audioSource.PlayOneShot(missClips[Random.Range(0, 3)]);
        }

        HandelSlashEffect(slash3, attack3BoxCastPosition.position);

    }

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
            obj.transform.localScale = new Vector3(-2,2,2);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 5;
        Destroy(obj, 0.5f);
    }

    //private void Update()
    //{
    //    VisualizeBoxCast(attack3BoxCastPosition.position, new Vector2(1.75f, 0.35f), transform.right, distance);
    //}
}
