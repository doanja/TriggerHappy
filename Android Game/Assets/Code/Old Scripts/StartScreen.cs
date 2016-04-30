using UnityEngine;

/*
* Resource: https://www.youtube.com/watch?v=lHb213yRP-Y&index=33&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n
*
* TODO: This class adds listeners to the Start Screen of the game. Upon clicking the "Start" button,
* the user will be transitioned to the "Level Select Screen" where the user will have to choose an 
* availible level.
*/
public class StartScreen : MonoBehaviour {

    public string LevelName; // the next screen transitioned too
	
	// Update is called once per frame
	public void Update () {
        if (!Input.GetMouseButtonDown(0)) // listens for any sort of click on the start screen 
            return;                         // TODO: implement a start button via GameObject + listen for a click on that

        GameManager.Instance.Reset(); // erase all points accumulated by the player
        Application.LoadLevel(LevelName); // loads the scene
	}
}
