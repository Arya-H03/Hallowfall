using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[System.Serializable]
public class EnemyMovementHandler : MonoBehaviour, IMoveable, IInitializeable<EnemyController>
{
    private EnemyController enemyController;
    private Transform enemyTransform;

    private FlowFieldManager flowFieldManager;
    private ZoneManager zoneManager;

    EnemySignalHub signalHub;

    private Vector2 nextMoveDir = Vector2.zero;
    Vector3 lastPos = Vector3.zero;

    private Cell currentCell;
    private Cell lastCell;

    private bool hasNotBeenOnFlow = true;
    private readonly float flowRequestDelay = 0.3f;
    private float flowRequestTimer;

    public Rigidbody2D Rb { get; set; }
    public Vector2 FaceDirection { get; set; }

  
    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        this.enemyTransform = enemyController.transform;
        Rb = enemyController.Rb;
        signalHub = enemyController.SignalHub;

        flowFieldManager = FlowFieldManager.Instance;
        zoneManager = ZoneManager.Instance;

        flowRequestTimer = flowRequestDelay;

        lastPos = enemyTransform.position;
    }

    public void MoveToPlayer(float speed)
    {
        FindNextMovementDirection();
        Move(nextMoveDir, speed);
        TryToTurn(enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos());
    }

    private void FindNextMovementDirection()
    {
        flowRequestTimer += Time.deltaTime;
        if (flowRequestTimer >= flowRequestDelay || hasNotBeenOnFlow)
        {
            lastCell = zoneManager.FindCurrentCellFromWorldPos(lastPos);
            currentCell = zoneManager.FindCurrentCellFromWorldPos(enemyController.transform.position);

            Vector2 newDir = flowFieldManager.RequestNewFlowDir(currentCell, lastCell);


            if (newDir != Vector2.zero)
            {
                nextMoveDir = newDir;
                lastPos = enemyTransform.position;
                hasNotBeenOnFlow = false;
                flowRequestTimer = 0;
            }
        }
    }
    public void Move(Vector2 movementDirection, float speed)
    {
        Vector2 targetPos = Rb.position + (movementDirection * speed * Time.deltaTime);
        Vector2 smoothPos = Vector2.Lerp(Rb.position, targetPos, 1f);
        Rb.MovePosition(smoothPos);
    }

    public void StopMove()
    {
        nextMoveDir = Vector2.zero;
        Rb.linearVelocity = Vector2.zero;
    }


    public void TryToTurn(Vector2 direction)
    {
        int xDir = direction.x >= 0 ? -1 : 1;
        if (FaceDirection.x != xDir)
        {
            signalHub.OnEnemyTurn?.Invoke(xDir);
            FaceDirection = new Vector2(xDir,0);
            this.transform.localScale = new Vector3(xDir, 1, 1);
        }
    }

    public bool IsCurrentCellBlockedByEnemies()
    {
        if(hasNotBeenOnFlow) return false;
        else if(!hasNotBeenOnFlow && currentCell.FlowVect == Vector2.zero) return true;
       
        return false;
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
