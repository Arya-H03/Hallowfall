using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BaseBlockInteractionState 
{
    private PlayerController playerController;
    private CCoroutineRunner coroutineRunner;
    protected  BlockTypeEnum blockType;

    public BaseBlockInteractionState(PlayerController playerController, CCoroutineRunner coroutineRunner, BlockTypeEnum blockType)
    {
        this.playerController = playerController;
        this.coroutineRunner = coroutineRunner;
        this.blockType = blockType;
    }

    public abstract void OnEnterBlock();
    public abstract void OnExitBlock();
    public abstract void OnStayBlock();
}
