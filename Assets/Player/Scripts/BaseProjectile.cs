using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;

    protected Vector2 startVel;

    protected Rigidbody2D rb;

    public float Damage { get => damage; set => damage = value; }
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        SetSpawnVelocity(startVel);
    }

    private void SetSpawnVelocity(Vector2 vel)
    {

        rb.velocity += vel;
        
    }
    public void SetProjectileCourse(GameObject target)
    {
        startVel = (target.transform.position - this.transform.position).normalized;
        int dirX = startVel.x < 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.transform.localScale.x * dirX, this.transform.localScale.y, this.transform.localScale.z);
    }
}
