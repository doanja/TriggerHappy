using UnityEngine;

/*
* Resource: 
*
* This class is used to determine the camera's position and limits the camera
* with certain boundaries based on GameObjects. If the camera object collides
* with the camera boundaries, then it will not move past the boundaries.
*/
public class CameraController : MonoBehaviour {
    public Transform Player;        // the Player Object's position
    public BoxCollider2D Bounds;    // the camera's boundary

    // Adjusts the rate of which the camera is moved
    public Vector2 
        Margin,
        Smoothing;

    // Camera's minimum and maximum boundaries
    private Vector3
        _min,
        _max;

    public bool IsFollowing { get; set; }

    // Use this for initialization
    public void Start()
    {
        _min = Bounds.bounds.min;
        _max = Bounds.bounds.max;
        IsFollowing = true;
    }

    // Update is called once per frame
    public void Update()
    {
        // X and Y-Coordinates of the Camera
        var x = transform.position.x;
        var y = transform.position.y;

        // Camera will follow the player's position
        if (IsFollowing)
        {
            if (Mathf.Abs(x - Player.position.x) > Margin.x)
                x = Mathf.Lerp(x, Player.position.x, Smoothing.x * Time.deltaTime);

            if (Mathf.Abs(y - Player.position.y) > Margin.y)
                y = Mathf.Lerp(y, Player.position.y, Smoothing.y * Time.deltaTime);
        }

        // Calculates the half width of the camera's size
        var cameraHalfWidth = GetComponent<Camera>().orthographicSize * ((float) Screen.width / Screen.height);

        // Sets the camera's size
        x = Mathf.Clamp(x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        y = Mathf.Clamp(y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);

        // Updates the camera's position
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
