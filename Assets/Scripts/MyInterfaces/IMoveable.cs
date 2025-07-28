using UnityEngine;

public interface IMoveable
{
    public Rigidbody2D Rb { get; set; }
    public Vector2 FaceDirection { get; set; }

    public void Move(Vector2 velocity, float speed);
    public void CheckForFacingDirection(Vector2 direction); 
}
