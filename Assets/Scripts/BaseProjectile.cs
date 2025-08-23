using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private string targetTag;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    public string TargetTag { get => targetTag; set => targetTag = value; }
    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public float LifeTime { get => lifeTime; set => lifeTime = value; }
    public Rigidbody2D RB { get; set; }
 

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Destroy(LifeTime);   
    }

    public void SetVelocity(Vector2 vel)
    {
        RB.linearVelocity = vel * Speed;   
    }

    public void ResetVelocity()
    {

        RB.linearVelocity =Vector2.zero;

    }

    public void Destroy(float lifetime)
    {
        Destroy(gameObject,lifetime);
    }
    public void SetProjectileCourseToTarget(GameObject target)
    {
        ResetVelocity();
        Vector3 targetCenter = target.transform.position;
        Vector3 vel = (targetCenter - this.transform.position).normalized;
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        if (angle < -90 || angle > 90)
        {
            angle += 180;
        }

        int dirX = vel.x < 0 ? 1 : -1;
        this.transform.localScale = new Vector3(this.transform.localScale.x * dirX, this.transform.localScale.y, this.transform.localScale.z);
        this.transform.rotation = Quaternion.Euler(0,0,angle);
        SetVelocity(vel);
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
        SetVelocity(vel);
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
        SetVelocity(vel);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TargetTag))
        {
            collision.gameObject.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = this.Damage });
            Destroy(0);
        }
    }

}
