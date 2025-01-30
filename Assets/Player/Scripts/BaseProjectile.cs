using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;

    protected int startDirection;

    protected Rigidbody2D rb;

    public float Damage { get => damage; set => damage = value; }
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        SetSpawnVelocity(startDirection);
    }

    private void SetSpawnVelocity(int dir)
    {

        rb.velocity = new Vector2(dir * speed, 0);
        
    }

    public void FindStartDirection(Vector3 parentScale)
    {
        startDirection = parentScale.x < 0 ? 1 : -1;

        this.transform.localScale = new Vector3(-this.transform.localScale.x * startDirection, this.transform.localScale.y, this.transform.localScale.z);


    }
}
