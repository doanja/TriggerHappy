using UnityEngine;

/*
* Resource: https://www.youtube.com/watch?v=Wrelb5WBnoQ&index=18&list=PLiyfvmtjWC_Up8XNvM3OSqgbJoMQgHkVz
*
* Class to display buttons and hide/show the PauseMenu canvas overlay.
*/
public class PauseMenu : MonoBehaviour {

    public string levelSelect;          // name of the level select screen
    public string mainMenu;             // name of the main menu scene  
    public GameObject pausedMenuCanvas;	// instance of the pausedMenuCanvas
	
    void Start()
    {
         pausedMenuCanvas.SetActive(false);  // hides the pause menu canvas
         Time.timeScale = 1f;                // reverts time
    }
   
    // Handles the Resume button
    public void Resume()
    {
        pausedMenuCanvas.SetActive(false);  // hides the pause menu canvas
        Time.timeScale = 1f;                // reverts time
    }

    // Handles the LevelSelect button
    public void LevelSelect()
    {
        Application.LoadLevel(levelSelect);
    }

    // Handles the MainMenu button
    public void Quit()
    {
        Application.LoadLevel(mainMenu);
    }

    // Handles the Pause button
    public void TouchPause()
    {        
        pausedMenuCanvas.SetActive(true);   // shows the pause menu canvas
        Time.timeScale = 0f;                // freezes time        
    }
}
