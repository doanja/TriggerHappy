using UnityEngine;

public class IceController2D : MonoBehaviour {

    private Player player;

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Handles collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        player.onIce = true;
    }

    // Handles collision
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        player.onIce = false;
    }
}
