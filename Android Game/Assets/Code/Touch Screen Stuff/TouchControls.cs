using UnityEngine;

/*
* Resource: https://www.youtube.com/watch?v=qvJCjy8vIrY&index=29&list=PLiyfvmtjWC_Up8XNvM3OSqgbJoMQgHkVz
*
* Class invokes functions within the Player and PauseMenu classes when the UI buttons
* and UI images are selected in game by the user.
*/
public class TouchControls : MonoBehaviour {

    // Reference Objects
    private Player thePlayer;
    private PauseMenu thePauseMenu;

	// Use this for initialization
	void Start () {
        thePlayer = FindObjectOfType<Player>();
        thePauseMenu = FindObjectOfType<PauseMenu>();       
	}
	
    /*
    * @param horizontalInput, 
    * Moves the Player left
    */
    public void LeftArrow(int horizontalInput)
    {
        thePlayer.hInput = horizontalInput;
    }

    /*
    * @param horizontalInput,
    * Moves the Player right
    */
    public void RightArrow(int horiztonalInput)
    {
        thePlayer.hInput = horiztonalInput;
    }

    /*
    * @param verticalInput,
    * Moves the Player up if they're on the ladder,
    * otherwise causes the Player to jump
    */
    public void UpArrow(int verticalInput)
    {
        thePlayer.TouchJump();
        thePlayer.vInput = verticalInput;
    }

    /*
    * @param verticalInput,
    * Moves the Player down if they're on the ladder
    */
    public void DownArrow(int verticalInput)
    {
        thePlayer.vInput = verticalInput;
    }

    /*
    * Defaults input when no buttons are pressed
    */
    public void UnpressedArrow()
    {
        thePlayer.MoveHorizontal(0);
        thePlayer.MoveVertical(0);
    }

    /*
    * Player shoots
    */
    public void ShootButton()
    {
        thePlayer.TouchShoot();
    }

    /*
    * Player jumps
    */
    public void JumpButton()
    {
        thePlayer.TouchJump();       
    }

    /*
    * Shows the pause menu
    */
    public void PauseMenu()
    {
        thePauseMenu.TouchPause();
    }
}
