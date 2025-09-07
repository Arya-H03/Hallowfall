using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerShadowStrikeAbility : MonoBehaviour, IAbility,IUpgradeableAbility
{
    private CDetector cDetector;
    private CSpawner cSpawner;

    private PlayerController playerController;
    private LayerMask layerMask;
    private string enemyTag;

    private float detectionRadius;
    private GameObject shadowClonePrefab;
    private int cycleDuration;
    private int spawnCount;
    private int shadowCloneDamage;

    public PlayerAbilitySO AbilitySO { get; set; }
    public int CycleDuration { get => cycleDuration; set => cycleDuration = value; }
    public int SpawnCount { get => spawnCount; set => spawnCount = value; }
    public int ShadowCloneDamage { get => shadowCloneDamage; set => shadowCloneDamage = value; }

    private void Awake()
    {
        Init();
    }
    public void InjectReferences(PlayerController controller, PlayerAbilitySO abilitySO)
    {
        this.playerController = controller;
        AbilitySO = abilitySO;

        if(AbilitySO is PlayerShadowStrikeSO shadowStrikeSO)
        {
            this.detectionRadius = shadowStrikeSO.detectionRadius;
            this.shadowClonePrefab = shadowStrikeSO.shadowClonePrefab;
            this.shadowCloneDamage = shadowStrikeSO.shadowCloneDamage;
            this.cycleDuration = shadowStrikeSO.cycleDuration;
            this.ShadowCloneDamage = shadowStrikeSO.shadowCloneDamage;
        }
        

        layerMask = playerController.PlayerConfig.enemyMask;
        enemyTag = playerController.EnemyTag;
        
    }
    public void Init()
    {
        cDetector = GetComponentInChildren<CDetector>();
        MyUtils.ValidateFields(this, cDetector, "CDetector");

        cSpawner = GetComponentInChildren<CSpawner>();
        MyUtils.ValidateFields(this, cSpawner, "CSpawner");
    }

    public void Perform()
    {
        StartCoroutine(ShadowStrikeCoroutine());
    }

    public void ApplyUpgrade(IAbilityUpgrade upgrade)
    {
        upgrade.ApplyUpgradeLogicTo(this);
    }
    private IEnumerator ShadowStrikeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleDuration);

            List<GameObject> detectedEnemies = cDetector.DetectNearbyGameObjectTargets(enemyTag,playerController.GetPlayerPos(),layerMask,detectionRadius);

            if (detectedEnemies.Count >= 1)
            {
                for (int i = 0; i < SpawnCount; ++i)
                {
                    int randomIndex = Random.Range(0, detectedEnemies.Count);
                    GameObject selectedEnemy = detectedEnemies[randomIndex];

                    GameObject shadowCloneGO = cSpawner.Spawn(shadowClonePrefab, selectedEnemy.transform.position, Quaternion.identity);
                    ShadowClone shadowClone = shadowCloneGO.GetComponent<ShadowClone>();
                    shadowClone.EnemyTarget = selectedEnemy;
                    shadowClone.Damage = ShadowCloneDamage;
                }
            }

           
        }
    }
}


