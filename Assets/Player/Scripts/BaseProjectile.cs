using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime = 2;

    protected Rigidbody2D rb;

    public float Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {

        StartCoroutine(DestroyProjectile());
    }

    private void ChangeVelocity(Vector2 vel)
    {

        rb.linearVelocity += vel * Speed;
        
    }

    private void ResetVelocity()
    {

        rb.linearVelocity =Vector2.zero;

    }
    public void SetProjectileCourseToTarget(GameObject target)
    {
        ResetVelocity();
        Vector3 targetCenter = target.transform.position + new Vector3(0, target.GetComponent<SpriteRenderer>().bounds.size.y / 2);
        Vector3 vel = (targetCenter - this.transform.position).normalized;
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        if (angle < -90 || angle > 90)
        {
            angle += 180;
        }

        int dirX = vel.x < 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.transform.localScale.x * dirX, this.transform.localScale.y, this.transform.localScale.z);
        this.transform.rotation = Quaternion.Euler(0,0,angle);
        ChangeVelocity(vel);
    }

    public void SetProjectileCourseForward(GameObject spawnerGO)
    {
        ResetVelocity();
        Vector2 vel = Vector2.zero;
        if (spawnerGO.transform.localScale.x == 1)
        {
            vel = new Vector2(1, 0);
        }
        if (spawnerGO.transform.localScale.x == -1)
        {
            vel = new Vector2(-1, 0);
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }
        ChangeVelocity(vel);
    }

    public void SetProjectileCourseToCursor()
    {
        ResetVelocity();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 vel = (mousePos - this.transform.position).normalized;
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        if (angle > -90 && angle < 90)
        {
           angle += 180;
        }
        int dirX = vel.x < 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.transform.localScale.x * dirX, this.transform.localScale.y, this.transform.localScale.z);
        this.transform.rotation = Quaternion.Euler(0, 0, angle);
        ChangeVelocity(vel);
        
    }

    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(this.gameObject);
    }

}
