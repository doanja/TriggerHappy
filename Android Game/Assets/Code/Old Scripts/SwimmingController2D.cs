using UnityEngine;

public class SwimmingController2D : MonoBehaviour {

    private Player player;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Handles collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        //player.inWater = true;      
    }

    // Handles collision
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        //player.inWater = false;        
    }
}
