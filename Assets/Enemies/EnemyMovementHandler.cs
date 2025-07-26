using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class EnemyMovementHandler : MonoBehaviour
{


    private EnemyController enemyController;

    private int currentDir = 0;

    private float distanceToTarget;
    private Vector3 nextDir = Vector3.zero;
    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public int CurrentDir { get => currentDir; set => currentDir = value; }

    Vector3 lastPos = Vector3.zero;
    private float flowRequestDelay = 0.15f;
    private float flowRequestTimer;
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();

    }

    private void Start()
    {
        flowRequestTimer = flowRequestDelay;
    }
    //public void MoveToLocation(Vector3 startPoint, Vector3 endPoint, float speed)
    //{

    //    Vector3 direction = endPoint - startPoint;
    //    TurnEnemy(direction);

    //}

    public void MoveToPlayer(float speed)
    {
        if (flowRequestTimer >= flowRequestDelay)
        {
            Vector3 currentPos = enemyController.transform.position;
            nextDir = FlowFieldManager.Instance.RequestNewFlowDir(currentPos, lastPos).normalized;
            lastPos = currentPos;
            flowRequestTimer = 0;

        }
        flowRequestTimer += Time.deltaTime;
        
        transform.position += speed * Time.deltaTime * nextDir;
        FaceMovementDirection(nextDir);

       
    }


    public void FaceMovementDirection(Vector3 movementVector)
    {
        Vector3 movDir = enemyController.PlayerController.GetPlayerCenter() - enemyController.GetEnemyCenter();
        int xDir = movDir.x >= 0 ? -1 : 1;


        Vector3 canvasScale = enemyController.WorldCanvas.localScale;
        enemyController.WorldCanvas.localScale = new Vector3(xDir * Mathf.Abs(canvasScale.x), canvasScale.y, canvasScale.z);


        if (CurrentDir != xDir)
        {
            CurrentDir = xDir;
            this.transform.localScale = new Vector3(xDir, 1, 1);
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
        if (direction.x >= 0)
        {
            enemyController.WorldCanvas.localScale = new Vector3(-Math.Abs(vec.x), vec.y, vec.z);
            if (CurrentDir != -1)
            {
                CurrentDir = -1;
                this.transform.localScale = new Vector3(CurrentDir, 1, 1);
            }
        }

    }

    public void StartRunningSFX(AudioClip groundSFX, AudioClip grassSFX, AudioClip woodSFX)
    {

        switch (enemyController.CurrentFloorType)
        {
            case FloorTypeEnum.Ground:
                AudioManager.Instance.PlaySFX(groundSFX, enemyController.transform.position, 1);
                break;
            case FloorTypeEnum.Grass:
                AudioManager.Instance.PlaySFX(grassSFX, enemyController.transform.position, 1);
                break;
            case FloorTypeEnum.Stone:
                AudioManager.Instance.PlaySFX(woodSFX, enemyController.transform.position, 1);
                break;
        }

    }

    public void StopRunningSFX(AudioSource audioSource)
    {
        AudioManager.Instance.StopAudioSource(audioSource);
    }


}
