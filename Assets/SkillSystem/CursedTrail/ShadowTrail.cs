using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ShadowTrail : MonoBehaviour
{
    private int damage;

    [SerializeField] LayerMask layerMask;

    private Coroutine damageCoroutine;
    public void Init(int damage)
    {
        this.damage = damage;
   
        damageCoroutine = StartCoroutine(TryDamageTargets());
    }

    public void OnDestroy()
    {
        StopCoroutine(damageCoroutine);
    }
    private IEnumerator TryDamageTargets()
    {
        while (true)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, this.transform.localScale.x / 2, transform.forward, 10f, layerMask);

            foreach (RaycastHit2D hit in hits)
            {

                if (hit.collider != null && hit.collider.transform.parent.TryGetComponent<EnemyController>(out EnemyController enemyController))
                {
                    enemyController.GetComponent<IHitable>().HandleHit(new HitInfo
                    {
                        damage = damage,
                        canBeImmune = false,
                        canFlashOnHit = true,
                        canPlayAnimOnHit = false,
                        canPlaySFXOnHit = false,
                        canPlayVFXOnHit = false
                    });
                }
            }

            yield return new WaitForSeconds(0.75f);
        }

    }
}
