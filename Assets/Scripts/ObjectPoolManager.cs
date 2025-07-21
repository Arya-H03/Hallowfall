using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;

    [SerializeField] ObjectPool essencePool;
    [SerializeField] ObjectPool skullPool;

    [SerializeField] ObjectPool arsonistPool;
    [SerializeField] ObjectPool sinnerPool;
    [SerializeField] ObjectPool necroPool;
    [SerializeField] ObjectPool revenantPool;

    public static ObjectPoolManager Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject gameObject = new GameObject("ObjectPoolManager");
                gameObject.AddComponent<ObjectPoolManager>();
            }
            return instance;
        }
    }
    public ObjectPool EssencePool { get => essencePool;}
    public ObjectPool SkullPool { get => skullPool; }
    public ObjectPool ArsonistPool { get => arsonistPool; }
    public ObjectPool SinnerPool { get => sinnerPool; }
    public ObjectPool NecroPool { get => necroPool; }
    public ObjectPool RevenantPool { get => revenantPool; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public void GenerateEnemyPools()
    {
        arsonistPool.GeneratePool();
        sinnerPool.GeneratePool();
        revenantPool.GeneratePool();
        necroPool.GeneratePool();
    }

}
