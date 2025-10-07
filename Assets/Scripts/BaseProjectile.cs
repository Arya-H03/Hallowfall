using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private string targetTag;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float homingTime;
    [SerializeField] private float angleOffSet;

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
        Vector3 targetPos = target.transform.position;
        Vector3 dir = (targetPos - this.transform.position).normalized;
        RotateProjectileToDirection(dir);
       
        RB.linearVelocity = dir.normalized * Speed;
    }

    public void SetProjectileCourseToDir(Vector3 dir)
    {
        RotateProjectileToDirection(dir);
        RB.linearVelocity = dir.normalized * Speed;
    }

    public void SetProjectileCourseToRandomDir()
    {
        Vector3 dir = new Vector3(Random.Range(-1, 1.1f), Random.Range(-1, 1.1f), 0).normalized;
        RotateProjectileToDirection(dir);
        RB.linearVelocity = dir.normalized * Speed;
    }

    public void SetProjectileCourseToRandomOffSet(Transform target)
    {
        Vector3 dir = (target.transform.position - this.transform.position).normalized;
        float xOffset = dir.x >= 0 ? Random.Range(4, 12f) : Random.Range(-12f, -4);
        float yOffset = dir.y >= 0 ? Random.Range(4, 12f) : Random.Range(-12f, -4);
        Vector3 offSet = new Vector3(xOffset, yOffset);

        RotateProjectileToDirection(dir + offSet);
        RB.linearVelocity = (dir+ offSet ).normalized * Speed;
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

    private IEnumerator HomingMovementCoroutine(GameObject targetGO, float initialNoneHomingDuration, float homingDuration)
    {
        SetProjectileCourseToRandomOffSet(targetGO.transform);
       
        yield return new WaitForSeconds(initialNoneHomingDuration);

        Vector3 dir = RB.linearVelocity.normalized;

        float t = 0;
        while(t <= homingDuration)
        {
            Vector3 targetDir = (targetGO.transform.position - transform.position).normalized;

            dir = Vector3.Lerp(dir, targetDir, Time.fixedDeltaTime * 5f).normalized; 
            Vector2 newPos = this.transform.position + dir * speed * Time.fixedDeltaTime;

            RB.MovePosition(newPos);

            RotateProjectileToDirection(dir);
           
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        Vector3 endPos = this.transform.position + dir * 20;
        dir = (endPos - this.transform.position).normalized;

        SetProjectileCourseToDir(dir);

     
    }
    public void SetProjectileToHomingMovement(GameObject targetGO, float initialNoneHomingDuration, float homingDuration)
    {
        StartCoroutine(HomingMovementCoroutine(targetGO,initialNoneHomingDuration,homingDuration));   
    }
    private void RotateProjectileToDirection(Vector3 dir)
    {
        float newAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, newAngle + angleOffSet);
    }

    private IEnumerator CurvedMovementCoroutine(GameObject targetGO)
    {
        Vector3 start = this.transform.position;
        Vector3 target = targetGO.transform.position;

        //Vector3 curvePoint = new Vector3(
        //    Random.Range(start.x + start.x + (target.x - start.x) / 0.25f, start.x + (target.x - start.x) / 0.75f),
        //    Random.Range(start.y + (target.y - start.y) / 0.25f, start.y + (target.y - start.y) / 0.75f),
        //    0f
        //);
        float d = Vector3.Distance(start, target) / 2;
        float e = start.y - target.y;

        Vector3 curvePoint = new Vector3(
            curvePoint.x = start.x + Mathf.Sqrt((e * e) + (d * d)),
             curvePoint.y = start.y,
            0f
        );
        //curvePoint.y = start.y;
        //curvePoint.x =  start.x + Mathf.Sqrt((e * e) + (d * d));

        Vector3 controlPoint = CurveUtils.ControlPointForMidpoint(start, target, curvePoint);

        float length = CurveUtils.ApproximateLength(start, controlPoint, target, 30);
        float t = 0f;


        while (t < 1f)
        {    
            Vector3 targetPos = CurveUtils.QuadraticBezier(start, controlPoint, target, t);
            transform.position = targetPos;

            Vector3 tangent = CurveUtils.QuadraticBezierTangent(start, controlPoint, target, t).normalized;
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 270f);
            
            t += (Time.deltaTime * speed) / length;
            yield return null;
        }

        transform.position = target;

        Vector3 endDir = CurveUtils.QuadraticBezierTangent(start, controlPoint, target, 1f).normalized;
        Vector3 endPoint = target + endDir * 20;

        float distance = Vector3.Distance(transform.position, endPoint);

        while (distance > 0f)
        {
            Vector3 move = Time.deltaTime * speed * endDir;
            transform.position += move;

            // Optional: keep rotation aligned
            float angle = Mathf.Atan2(endDir.y, endDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 270f);

            distance -= move.magnitude;
            yield return null;
        }

        transform.position = endPoint;

    }

    public void SetProjectileToCurvedMovement(GameObject target)
    {
        StartCoroutine(CurvedMovementCoroutine(target));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TargetTag))
        {
            collision.gameObject.GetComponent<IHitable>().HandleHit(new HitInfo { damage = this.Damage, canBeImmune = false });
            Destroy(0);
        }
    }

}
