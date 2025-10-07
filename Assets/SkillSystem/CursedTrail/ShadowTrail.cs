using System.Collections;
using System.Collections.Generic;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
public class ShadowTrail : MonoBehaviour
{
    private EntityController owner;
    private int damage;
    private EntityTypeEnum targetEntityType;

    [SerializeField] LayerMask layerMask;

    private Coroutine damageCoroutine;
    public void Init(EntityController owner, int damage, EntityTypeEnum ownerEntityType)
    {
        this.owner = owner;
        this.damage = damage;
        this.targetEntityType = (ownerEntityType == EntityTypeEnum.player) ? EntityTypeEnum.enemy : EntityTypeEnum.player;

        damageCoroutine = StartCoroutine(TryDamageTargets());
    }

    public void OnDestroy()
    {
        StopCoroutine(damageCoroutine);
    }
    private IEnumerator TryDamageTargets()
    {
        switch(targetEntityType)
        {
            case EntityTypeEnum.player:
                while (true)
                {
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, this.transform.localScale.x / 2, transform.forward, 10f, layerMask);
                    
                    foreach (RaycastHit2D hit in hits)
                    {

                        if (hit.collider != null && hit.collider.transform.TryGetComponent<PlayerController>(out PlayerController playerController))
                        {
                            playerController.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = damage, isImmuneable = false });
                        }
                    }
                }
            case EntityTypeEnum.enemy:
                while (true)
                {
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, this.transform.localScale.x / 2, transform.forward, 10f, layerMask);
           
                    foreach (RaycastHit2D hit in hits)
                    {

                        if (hit.collider != null && hit.collider.transform.parent.TryGetComponent<EnemyController>(out EnemyController enemyController))
                        {
                            enemyController.GetComponent<IHitable>().HandleHit(new HitInfo { Damage = damage, HitSfx = HitSfxType.none, isImmuneable = false });
                        }
                    }

                    yield return new WaitForSeconds(0.75f);
                }
        }
      
    }
}
