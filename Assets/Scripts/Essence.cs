using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    public bool isPlayerInRange = false;
    public bool canBePicked = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
       StartCoroutine(LaunchEssence());
    }

    private void Update()
    {
        if (isPlayerInRange)
        {
            SnapToPlayer(player);
        }
    }


    private IEnumerator LaunchEssence()
    {
        float xDir = Random.Range(-3, 3.1f);
        rb.velocity = new Vector2(rb.velocity.x + xDir, rb.velocity.y + 7);
        yield return new WaitForSeconds(1.5f);
        rb.velocity = new Vector2(rb.velocity.x - xDir, rb.velocity.y - 7);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePicked)
        {
            collision.GetComponent<Player>().OnEssencePickUp();
            Destroy(this.gameObject);
        }
    }

    public void SnapToPlayer(GameObject player )
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 3f * Time.deltaTime);
    }



}
