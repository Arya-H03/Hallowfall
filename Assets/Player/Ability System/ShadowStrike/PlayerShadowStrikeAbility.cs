using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerShadowStrikeAbility : MonoBehaviour, IAbility,IUpgradeableAbility
{
    private CDetector cDetector;
    private CSpawner cSpawner;

    [SerializeField] private GameObject shadowClonePrefab;

    [SerializeField] private int cycleDuration;
    [SerializeField] private int spawnCount;
    [SerializeField] private int shadowCloneDamage = 50;

    public int CycleDuration { get => cycleDuration; set => cycleDuration = value; }
    public int SpawnCount { get => spawnCount; set => spawnCount = value; }
    public int ShadowCloneDamage { get => shadowCloneDamage; set => shadowCloneDamage = value; }

    private void Awake()
    {
        Init();
    }
    public void PassPlayerControllerRef(PlayerController controller)
    {

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
            yield return new WaitForSeconds(CycleDuration);

            List<GameObject> detectedEnemies = cDetector.DetectNearbyTargets();

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

//UPGRADES



