using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour {

    public Transform player;
    public float smoothrate = 0.5f;
    private Transform thisTransform;
    private Vector2 velocity;

	// Use this for initialization
	void Start () {
        thisTransform = transform;
        velocity = new Vector2(0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 newPos2D = Vector2.zero;
        newPos2D.x = Mathf.SmoothDamp(thisTransform.position.x, player.position.x, ref velocity.x, smoothrate);
        newPos2D.y = Mathf.SmoothDamp(thisTransform.position.y, player.position.y, ref velocity.y, smoothrate);

        // Update camera to player location
        Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, transform.position.z);
        transform.position = Vector3.Slerp(transform.position, newPos, Time.time);
    }

}
