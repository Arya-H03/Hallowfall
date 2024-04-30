using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetBoolForAnimation(string name, bool value)
    {
        animator.SetBool(name, value);
        Debug.Log(value);
    }

    public void SetTriggerForAnimation(string name)
    {
        animator.SetTrigger(name);

    }
}
