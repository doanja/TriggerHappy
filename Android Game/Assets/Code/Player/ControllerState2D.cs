public class ControllerState2D {

    // checks if there is anything colliding to the right of the player
    public bool IsCollidingRight { get; set; }

    // checks if there is anything colliding to the left of the player
    public bool IsCollidingLeft { get; set; }

    // checkes if the player has anything on top of them i.e. a ceiling
    public bool IsCollidingAbove { get; set; }

    // checks if the player is on the ground
    public bool IsCollidingBelow { get; set; }

    // is the player going down a slope
    public bool IsMovingDownSlope { get; set; }

    // is the player going up a slope
    public bool IsMovingUpSlope { get; set; }

    // is the player on the ground
    public bool IsGrounded { get { return IsCollidingBelow; } }

    // the angle of the slope
    public float SlopeAngle { get; set; }

    // checks if the player is colliding with anything in the 4 directions
    public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }

    // resets all values to false, and the slope angle to 0
    public void Reset()
    {
        IsMovingUpSlope =
            IsMovingDownSlope =
            IsCollidingLeft =
            IsCollidingRight =
            IsCollidingAbove =
            IsCollidingBelow = false;

        SlopeAngle = 0;
    }

    public override string ToString()
    {
        return string.Format(
            "(controller: r:{0} 1:{1} a:{2} b:{3} down-slope:{4} up-slope:{5} angle:{6}",
            IsCollidingRight,
            IsCollidingLeft,
            IsCollidingAbove,
            IsCollidingBelow,
            IsMovingDownSlope,
            IsMovingUpSlope,
            SlopeAngle);
    }
}
