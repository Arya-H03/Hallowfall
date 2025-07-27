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
    private Transform enemyTransform;
    private Rigidbody2D rb;
    private FlowFieldManager flowFieldManager;
    private ZoneManager zoneManager;

    private int currentDir = 0;
    private Vector2 nextMoveDir = Vector2.zero;
    Vector3 lastPos = Vector3.zero;

    private Cell currentCell;
    private Cell lastCell;

    private bool hasNotBeenOnFlow = true;
    [SerializeField] private float flowRequestDelay = 0.3f;
    private float flowRequestTimer;
    public int CurrentDir { get => currentDir; set => currentDir = value; }

    
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
       
    }

    private void Start()
    {
        enemyTransform = enemyController.transform;
        rb = enemyController.CollisionManager.Rb;
        flowFieldManager = FlowFieldManager.Instance;
        zoneManager = ZoneManager.Instance; 
        flowRequestTimer = flowRequestDelay;

        lastPos = enemyController.transform.position;
    }
   
    public void MoveToPlayer(float speed)
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

        Vector2 targetPos = rb.position + (nextMoveDir * speed * Time.deltaTime);
        Vector2 smoothPos = Vector2.Lerp(rb.position, targetPos, 1f); 
        rb.MovePosition(smoothPos);
        FaceMovementDirection();
  
    }

    public bool IsCurrentCellBlockedByEnemies()
    {
        if(hasNotBeenOnFlow) return false;
        else if(!hasNotBeenOnFlow && currentCell.FlowVect == Vector2.zero) return true;
       
        return false;
    }

    public void FaceMovementDirection()
    {
        Vector3 moveDir = enemyController.PlayerController.GetPlayerPos() - enemyController.GetEnemyPos();
        int xDir = moveDir.x >= 0 ? -1 : 1;
        //int xDir = movementDir.x >= 0 ? -1 : 1;

        Vector3 canvasScale = enemyController.WorldCanvas.localScale;
        enemyController.WorldCanvas.localScale = new Vector3(xDir * Mathf.Abs(canvasScale.x), canvasScale.y, canvasScale.z);


        if (CurrentDir != xDir)
        {
            CurrentDir = xDir;
            this.transform.localScale = new Vector3(xDir, 1, 1);
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
