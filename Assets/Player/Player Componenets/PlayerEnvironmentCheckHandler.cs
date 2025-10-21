
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[RequireComponent(typeof(CDetector))]
public class PlayerEnvironmentCheckHandler : MonoBehaviour
{
    private CDetector detector;

    [SerializeField] Transform groundCheckOrigin1;
    [SerializeField] Transform groundCheckOrigin2;

    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask interactionLayerMask;

    private PlayerController playerController;

    [SerializeField] private int detectionRadius = 4;
    [SerializeField] Color vignetteColorInForest;

    private NoneBlockState noneBlockState;
    private ForestBlockState forestBlockState;
    private GraveyardBlockState graveyardBlockState;
    public List<BlockTypeEnum> currentBlocks = new();
    public List<BlockTypeEnum> newBlocks = new();
    private Dictionary<BlockTypeEnum, BaseBlockInteractionState> blockStateDic = new();




    public CDetector Detector { get => detector; }
    public LayerMask EnemyLayerMask { get => enemyLayer; }

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        detector = GetComponent<CDetector>();
    }
    private void Start()
    {
        noneBlockState = new NoneBlockState(playerController, playerController.CoroutineRunner, BlockTypeEnum.none);
        forestBlockState = new ForestBlockState(playerController, playerController.CoroutineRunner, BlockTypeEnum.treeCluster, ZoneManager.Instance.GlobalTreeTilemap, detectionRadius, vignetteColorInForest);
        graveyardBlockState = new GraveyardBlockState(playerController, playerController.CoroutineRunner, BlockTypeEnum.graveCluster);

        blockStateDic.Add(BlockTypeEnum.none, noneBlockState);
        blockStateDic.Add(BlockTypeEnum.treeCluster, forestBlockState);
        blockStateDic.Add(BlockTypeEnum.graveCluster, graveyardBlockState);
        blockStateDic.Add(BlockTypeEnum.ritualCluster, noneBlockState);
        blockStateDic.Add(BlockTypeEnum.skillStatueCluster, noneBlockState);
        blockStateDic.Add(BlockTypeEnum.cryptCluster, noneBlockState);

    }
    private void FixedUpdate()
    {
        CheckForFloorType();
    }
    private void Update()
    {
        if (!playerController || playerController.IsDead || !ZoneManager.Instance) return;

        ChangeEnvironemntState();

        foreach (BlockTypeEnum blockType in currentBlocks)
        {
            blockStateDic[blockType].OnStayBlock();
        }
    }

    private void ChangeEnvironemntState()
    {

        foreach (Vector2Int vect in MyUtils.GetAllDirectionsVectorList())
        {
            Cell neighboor = ZoneManager.Instance.FindCurrentCellFromWorldPos(playerController.GetPlayerPos() + (Vector3Int)vect);
            newBlocks.Add(ZoneManager.Instance.GetCurrentZoneHandler().FindCellBlockType(neighboor));
        }
        Cell currentCell = ZoneManager.Instance.FindCurrentCellFromWorldPos(playerController.GetPlayerPos() - new Vector3(0, 0, 0));
        newBlocks.Add(ZoneManager.Instance.GetCurrentZoneHandler().FindCellBlockType(currentCell));

        foreach (var blockType in newBlocks.Except(currentBlocks))
        {
            currentBlocks.Add(blockType);
            blockStateDic[blockType].OnEnterBlock();
        }

        foreach (var blockType in currentBlocks.Except(newBlocks).ToList())
        {
            currentBlocks.Remove(blockType);
            blockStateDic[blockType].OnExitBlock();
        }

        newBlocks.Clear();

    }
    private void CheckForFloorType()
    {
        RaycastHit2D rayCast = Physics2D.Raycast(groundCheckOrigin1.transform.position, Vector2.down, 0.25f, groundLayer);

        if (rayCast && !rayCast.collider.CompareTag(playerController.CurrentFloorType.ToString()))
        {
            //playerController.StateMachine.PlayerRunState.StopRunningSFX();
            switch (rayCast.collider.tag)
            {
                case "Ground":
                    playerController.CurrentFloorType = FloorTypeEnum.Ground;
                    break;
                case "Stone":
                    playerController.CurrentFloorType = FloorTypeEnum.Stone;
                    break;
                case "Grass":
                    playerController.CurrentFloorType = FloorTypeEnum.Grass;
                    break;
            }

            if (playerController.StateMachine.CurrentStateEnum == PlayerStateEnum.Run)
            {
                //playerController.StateMachine.PlayerRunState.StartRunningSFX();
            }

        }
    }
    //private void CheckForInteractions()
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(headLevelCheckOrigin.position, new Vector2(playerController.gameObject.transform.localScale.x * 1, 0), 1f, interactionLayerMask);
    //    if(hit) 
    //    {
    //        switch(hit.collider.tag)
    //        {
    //            case "Statue":
    //                if(currentInteractable == null)
    //                {
    //                    currentInteractable = hit.transform.gameObject.GetComponent<IInteractable>();
    //                    currentInteractable.OnIntercationBegin();
    //                }
    //                break;
    //            default:
    //                Debug.Log("Other Tag: " + hit.collider.tag);
    //                break;
    //        }
    //    }

    //    else
    //    {
    //        if(currentInteractable != null)
    //        {
    //            currentInteractable.OnIntercationEnd();
    //            currentInteractable = null;
    //        }
    //    }
    //}


}
