using UnityEngine;

/*
* Resource: https://www.youtube.com/watch?v=lHb213yRP-Y&index=33&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n
*
* Handles level transition upon colliding with this GameObject.
*/
public class Portal : MonoBehaviour {

    private Player player;              // reference to the Player
    public bool playerInPortal;         // checks for Player collision with this GameObject    
    private EndLevelMenu endLevelMenu;  // the end level canvas

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<Player>();
        endLevelMenu = FindObjectOfType<EndLevelMenu>();
    }
	
	// Update is called once per frame
	void Update () {        
        if (playerInPortal)
        {                      
            endLevelMenu.ShowEndLevelMenu();
        }
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
