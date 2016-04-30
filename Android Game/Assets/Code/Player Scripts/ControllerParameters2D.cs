using System;
using UnityEngine;

[Serializable]
public class ControllerParameters2D {

    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    // MaxVelocity = MaxVelocity in X direction and MaxVelocity in Y direction
    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeLimit = 30; // angle allowed to travel (this case is 30 degrees)

    public float Gravity = -25f; // default gravity is -25f
    public JumpBehavior JumpRestrictions;
    public float JumpFrequency = .25f; // limits how often the player can jump
    public float JumpMagnitude = 12;
}
