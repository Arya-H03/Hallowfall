using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyMovement : MonoBehaviour
{
   
   
    private EnemyController enemyController;

    private int currentDir = 0;

    private float distanceToTarget;

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public int CurrentDir { get => currentDir; set => currentDir = value; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

 
    public void MoveTo(Vector3 startPoint, Vector3 endPoint, float speed)
    {
       
        Vector3 direction = endPoint - startPoint;

        enemyController.NavAgent.speed = speed;
        enemyController.NavAgent.SetDestination(endPoint);
        TurnEnemy(direction);
            
    }

    public int FindDirectionToPlayer()
    {
        if(enemyController.PlayerPos.x - enemyController.transform.position.x >= 0)
        {
            return 1;
        }
        else
        {         
            return -1;
        }
    }

    private void TurnEnemy(Vector3 direction)
    {
        
        if(direction.x < 0 && CurrentDir != 1)
        {
            CurrentDir = 1;
            this.transform.localScale = new Vector3(CurrentDir, 1, 1);   
            //OnEnemyBeginTurning(1);

        }
        if(direction.x >= 0 && CurrentDir != -1)
        {
            CurrentDir = -1;
            this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            //OnEnemyBeginTurning(-1);
        }
    }

    public void StartRunningSFX(AudioSource audioSource,AudioClip groundSFX, AudioClip grassSFX, AudioClip woodSFX)
    {
        
        switch (enemyController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                AudioManager.Instance.PlaySFX(audioSource, groundSFX);
                break;
            case FloorTypeEnum.Grass:
                AudioManager.Instance.PlaySFX(audioSource, grassSFX);
                break;
            case FloorTypeEnum.Wood:
                AudioManager.Instance.PlaySFX(audioSource, woodSFX);
                break;
        }

    }

    public void StopRunningSFX(AudioSource audioSource)
    {
        AudioManager.Instance.StopAudioSource(audioSource);
    }

}
