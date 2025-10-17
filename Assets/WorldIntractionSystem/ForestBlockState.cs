using UnityEngine;

public class ForestBlockState : BaseBlockInteractionState
{
    public ForestBlockState(PlayerController playerController, CCoroutineRunner coroutineRunner, BlockTypeEnum blockType) : base(playerController, coroutineRunner, blockType)
    {
    }

    public override void OnEnterBlock()
    {
        Debug.Log("We in Forest");
    }

    public override void OnExitBlock()
    {
        Debug.Log("We out Forest");
    }
}
