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
    private ZoneManager zoneManager;
   
    private int currentDir = 0;

    private float distanceToTarget;

    public float DistanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    public int CurrentDir { get => currentDir; set => currentDir = value; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
       
    }

    private void Start()
    {
        zoneManager = ZoneManager.Instance;
    }


    //public void MoveToLocation(Vector3 startPoint, Vector3 endPoint, float speed)
    //{

    //    Vector3 direction = endPoint - startPoint;
    //    TurnEnemy(direction);

    //}

    public void MoveToPlayer(float speed)
    {
        if (zoneManager == null) return;

        ZoneData zoneData = zoneManager.FindCurrentZoneBasedOnWorldPos(enemyController.GetEnemyCenter());
        Cell currentCell = zoneData.ZoneHandler.CellGrid.GetCellFromWorldPos(enemyController.GetEnemyCenter());

        Vector3 flowVector = new Vector3(currentCell.FlowVect.x, currentCell.FlowVect.y, 0);
        //Vector2 separationVector = enemyController.EnemyEnvironenmentCheck.CalculateSeparationForce();

        // If there's no flow vector, randomly pick one from neighbors
        if (flowVector == Vector3.zero)
        {
            List<Cell> neighborCells = currentCell.GetAllNeighborCells();
            Vector2 newFlowVect = neighborCells[Random.Range(0, neighborCells.Count)].FlowVect;
            flowVector = new Vector3(newFlowVect.x, newFlowVect.y, 0);
        }

        if (flowVector != Vector3.zero)
            FaceMovementDirection(flowVector);

        Vector3 finalFlowVector = (flowVector /*+ (Vector3)separationVector*/).normalized;

       
       

        
        transform.position += speed * Time.deltaTime * finalFlowVector;
    }

    public void FaceMovementDirection(Vector3 movementVector)
    {
        Vector3 movDir = enemyController.PlayerController.GetPlayerCenter() - enemyController .GetEnemyCenter();
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
