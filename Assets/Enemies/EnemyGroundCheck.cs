using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{
    [SerializeField] Transform groundCheckOrigin1;
    [SerializeField] Transform groundCheckOrigin2;

    [SerializeField] LayerMask groundLayer;

    EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();  
    }
    private void FixedUpdate()
    {
        GroundCheck();
    }
    private void GroundCheck()
    {
        RaycastHit2D[] rayCasts = new RaycastHit2D[2];
        rayCasts[0] = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);
        rayCasts[1] = Physics2D.Raycast(groundCheckOrigin2.transform.position, Vector2.down, 0.25f, groundLayer);

        foreach (RaycastHit2D rayCast in rayCasts)
        {
            if (rayCast.collider != null)
            {
                switch (rayCast.collider.tag)
                {
                    case "Ground":
                        enemyController.CurrentFloorType = FloorTypeEnum.Ground;
                        break;
                    case "Wood":
                        enemyController.CurrentFloorType = FloorTypeEnum.Wood;
                        break;
                    case "Grass":
                        enemyController.CurrentFloorType = FloorTypeEnum.Grass;
                        break;
                }


            }
        }
    }
}
