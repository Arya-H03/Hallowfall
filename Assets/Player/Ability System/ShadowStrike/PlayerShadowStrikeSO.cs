using UnityEngine;

[CreateAssetMenu(fileName = "PlayerShadowStrikeSO", menuName = "Scriptable Objects/PlayerAbilitySO/PlayerShadowStrikeSO")]
public class PlayerShadowStrikeSO : PlayerAbilitySO
{
    public GameObject shadowClonePrefab;

    public float detectionRadius = 7.5f;
    public int cycleDuration;
    public int spawnCount;
    public int shadowCloneDamage = 50;
}
