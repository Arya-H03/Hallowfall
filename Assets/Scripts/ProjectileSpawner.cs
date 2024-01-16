using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    public bool canShoot = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShootProjectileCoroutine(Vector3 destination,Vector3 start)
    {
        while(canShoot)
        {
            yield return new WaitForSeconds(1);
            Vector3 direction = destination - start;
            GameObject proj = Instantiate(projectile, this.transform.position, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().velocity = direction * 2;
            
        }
       

    }

    public void StartShooting(Vector3 destination, Vector3 start)
    {
        StartCoroutine(ShootProjectileCoroutine(destination, start));
    }
}
