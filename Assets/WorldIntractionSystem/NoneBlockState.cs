using UnityEngine;

public class NoneBlockState : BaseBlockInteractionState
{
    public NoneBlockState(PlayerController playerController, CCoroutineRunner coroutineRunner, BlockTypeEnum blockType) : base(playerController, coroutineRunner, blockType) { }

    public override void OnEnterBlock() { }


    public override void OnExitBlock() { }
   
}
