using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GraveyardBlockState : BaseBlockInteractionState
{
    public GraveyardBlockState(PlayerController playerController, CCoroutineRunner coroutineRunner, BlockTypeEnum blockType) : base(playerController, coroutineRunner, blockType)
    {
    }

    public override void OnEnterBlock()
    {
        PlayerCameraHandler.Instance.EnableFog(3);
    }

    public override void OnExitBlock()
    {
        PlayerCameraHandler.Instance.DisableFog(3);
    }

    public override void OnStayBlock()
    {
        return;
    }
}
