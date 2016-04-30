using UnityEngine;

/*
* This class that allows objects to paired with another object. This is mainly used by 
* the Player object, so that any object can follow it, without flipping the sprite.
*/
public class FollowObject : MonoBehaviour {

    public Vector2 Offset;      // the distance between this and the object this is following
    public Transform Following; // the object to follow

	// Update is called once per frame
	public void Update () {
        transform.position = Following.transform.position + (Vector3) Offset;
	}
}
