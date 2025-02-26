using System;
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

 
    public void MoveToLocation(Vector3 startPoint, Vector3 endPoint, float speed)
    {
       
        Vector3 direction = endPoint - startPoint;
        enemyController.NavAgent.speed = speed;
        enemyController.NavAgent.SetDestination(endPoint);
        TurnEnemy(direction);
            
    }

    public void MoveToPlayer(float speed)
    {

        Vector3 endPoint;
        if ((enemyController.transform.position.x - enemyController.Player.transform.position.x) > 0)
        {
            endPoint = enemyController.Player.transform.position + new Vector3(enemyController.AttackState.NextAttack.AttackRange, 0, 0);
        }
        else
        {
            endPoint = enemyController.Player.transform.position + new Vector3(-enemyController.AttackState.NextAttack.AttackRange, 0, 0);
        }
        enemyController.NavAgent.speed = speed;
        enemyController.NavAgent.SetDestination(endPoint);
        FacePlayer();

    }

    private int FindDirectionToPlayer()
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

    public void FacePlayer()
    {
        int xDir = FindDirectionToPlayer();
        Vector3 vec = enemyController.WorldCanvas.localScale;
        if (xDir < 0)
        {
            enemyController.WorldCanvas.localScale = new Vector3(Math.Abs(vec.x), vec.y, vec.z);
            if (CurrentDir != 1)
            {
                CurrentDir = 1;
                this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            }


        }
        if (xDir >= 0)
        {
            enemyController.WorldCanvas.localScale = new Vector3(-Math.Abs(vec.x), vec.y, vec.z);
            if (CurrentDir != -1)
            {
                CurrentDir = -1;
                this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            }
        }
    }

    private void TurnEnemy(Vector3 direction)
    {
        Vector3 vec = enemyController.WorldCanvas.localScale;
        if (direction.x < 0)
        {
            enemyController.WorldCanvas.localScale = new Vector3(Math.Abs(vec.x), vec.y, vec.z);
            if (CurrentDir != 1)
            {
                CurrentDir = 1;
                this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            }
           
 
        }
        if(direction.x >= 0)
        {
            enemyController.WorldCanvas.localScale = new Vector3(-Math.Abs(vec.x), vec.y, vec.z);
            if (CurrentDir != -1)
            {
                CurrentDir = -1;
                this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            }          
        }

    }

    public void StartRunningSFX(AudioSource audioSource,AudioClip groundSFX, AudioClip grassSFX, AudioClip woodSFX)
    {
        
        switch (enemyController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                AudioManager.Instance.PlaySFX(audioSource, groundSFX, 1);
                break;
            case FloorTypeEnum.Grass:
                AudioManager.Instance.PlaySFX(audioSource, grassSFX, 1);
                break;
            case FloorTypeEnum.Stone:
                AudioManager.Instance.PlaySFX(audioSource, woodSFX, 1);
                break;
        }

    }

    public void StopRunningSFX(AudioSource audioSource)
    {
        AudioManager.Instance.StopAudioSource(audioSource);
    }

}
