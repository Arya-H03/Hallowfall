using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "FlameImbueAbility", menuName = "FlameImbueAbility")]
public class FlameImbueAbility : BaseAbility
{
    public GameObject fireSlash1;
    public GameObject fireSlash2;


    public override void ApplyAbility()
    {
        PlayerSwordAttackState state = GameManager.Instance.Player.GetComponentInChildren<PlayerSwordAttackState>();
        state.SwordSwing1Event += FireImbueFirstSlash;
        state.SwordSwing2Event += FireImbueSecondSlash;
        LevelupManager.Instance.abilities.Remove(this);
    }

    private void FireImbueFirstSlash()
    {
        PlayerSwordAttackState state = GameManager.Instance.Player.GetComponentInChildren<PlayerSwordAttackState>();
        GameObject obj = Instantiate(fireSlash1, state.FirstSwingCenter.position, Quaternion.identity);

        Vector3 scale = GameManager.Instance.Player.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-2, 2, 2);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 5;
        Destroy(obj, 0.5f);

    }

    private void FireImbueSecondSlash()
    {
        PlayerSwordAttackState state = GameManager.Instance.Player.GetComponentInChildren<PlayerSwordAttackState>();
        GameObject obj = Instantiate(fireSlash2, state.FirstSwingCenter.position, Quaternion.identity);
        
        Vector3 scale = GameManager.Instance.Player.transform.localScale;

        Vector2 launchVec = Vector2.zero;
        if (scale.x == 1)
        {
            launchVec = new Vector2(1, 0);
        }
        if (scale.x == -1)
        {
            launchVec = new Vector2(-1, 0);
            obj.transform.localScale = new Vector3(-2, 2, 2);
        }
        obj.GetComponent<Rigidbody2D>().velocity = launchVec * 5;
        Destroy(obj, 0.5f);

    }
}
