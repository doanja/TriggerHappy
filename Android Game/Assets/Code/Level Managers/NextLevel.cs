using UnityEngine;

/*
* Alternative class to transition the player to a different level instead
* of showing the end level screen.
*/
public class NextLevel : MonoBehaviour {

    private Player player;              // reference to the Player
    public bool playerInPortal;         // checks for Player collision with this GameObject    
    public string levelToBeLoaded;      // name of the level to be loaded   

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();       
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInPortal)
            Application.LoadLevel(levelToBeLoaded);        
    }

    /*
   * @param other, the object that is colliding with this object
   * Checks to see if the player collided with this object
   */
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        playerInPortal = true;
    }

    /*
    * @param other, the object that is colliding with this object
    * Checks to see if the player no longer collided with this object
    */
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        playerInPortal = false;
    }
}
