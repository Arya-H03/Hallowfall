using UnityEngine;

public enum EntityTypeEnum
{
    player,
    enemy
}
public class EntityController : MonoBehaviour
{
    [SerializeField] protected EntityTypeEnum entityType;
    private bool isActive = false;
    public EntityTypeEnum EntityType { get => entityType; }
    public bool IsActive { get => isActive; set => isActive = value; }
}
