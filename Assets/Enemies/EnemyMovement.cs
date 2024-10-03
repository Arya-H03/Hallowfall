using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyMovement : MonoBehaviour
{
   
   
    private EnemyController enemyController;

    private int currentDir = 0;

    private float distanceToTarget;

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

 
    public void MoveTo(Vector2 startPoint, Vector2 endPoint, float speed)
    {
       
        Vector2 direction = endPoint - startPoint;
        transform.position = Vector2.MoveTowards(startPoint, new Vector2(endPoint.x,startPoint.y), speed * Time.deltaTime);
        TurnEnemy(direction);
            
    }

    public int FindDirectionToPlayer()
    {
        if(enemyController.player.gameObject.transform.position.x - enemyController.transform.position.x >= 0)
        {
            return 1;
        }
        else
        {         
            return -1;
        }
    }

    private void TurnEnemy(Vector2 direction)
    {
        
        if(direction.x < 0 && currentDir != 1)
        {
            currentDir = 1;
            this.transform.localScale = new Vector3(currentDir, 1, 1);   
            //OnEnemyBeginTurning(1);

        }
        if(direction.x >= 0 && currentDir != -1)
        {
            currentDir = -1;
            this.transform.localScale = new Vector3(currentDir, 1, 1);
            //OnEnemyBeginTurning(-1);
        }
    }

    private void OnEnemyBeginTurning(int dir)
    {
        currentDir = dir;
        
      
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
