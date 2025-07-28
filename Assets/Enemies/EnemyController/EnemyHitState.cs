//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyHitState : EnemyState
//{
//    private int damage;
//    private Vector2 hitPoint;
//    private HitSfxType hitType;
//    [SerializeField] private float hitDelay = 0.5f;

  
//    public int Damage { get => damage; set => damage = value; }
//    public Vector2 HitPoint { get => hitPoint; set => hitPoint = value; }
//    public HitSfxType HitType { get => hitType; set => hitType = value; }

//    private void Awake()
//    {
        
//    }
//    public EnemyHitState() : base()
//    {
//        stateEnum = EnemyStateEnum.Hit;

//    }
//    public override void EnterState()
//    {
//        StartCoroutine(EnemyHitCoroutine(damage, hitPoint, hitDelay, HitType));
//    }

//    public override void ExitState()
//    {
        
//    }

//    public override void FrameUpdate()
//    {


//    }

  

//}
