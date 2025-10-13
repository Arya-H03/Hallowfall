using System.Collections;
using Unity.AppUI.Core;
using UnityEngine;

public class Crow : MonoBehaviour
{
    [SerializeField] private float minSpeed = 4;
    [SerializeField] private float maxSpeed = 6;
    [SerializeField] private AudioClip [] crowSFX;

    private void Start()
    {
        StartCoroutine(MoveToRandomDirection());
        AudioManager.Instance.PlaySFX(crowSFX,this.transform.position,0.3f);
        Destroy(gameObject, 5);
        
    }
    private IEnumerator MoveToRandomDirection()
    {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.1f, 1f), 0);
        Vector3 destination = dir * 1000;
        Vector3 moveDir = (destination - transform.position).normalized;

        if (moveDir.x < 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        while (true)
        {
            transform.position += moveDir * speed * Time.deltaTime;
            yield return null;
        }
    }
}
