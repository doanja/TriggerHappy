using UnityEngine;

/*
* Resource: Adapted from PauseMenu.
*
* Handles functionality of the EndLevelMenu canvas.
*/
public class EndLevelMenu : MonoBehaviour {

    // For the buttons    
    private string currentLevel;    // current scene name
    public string levelSelect;      // level select scene name
    public string mainMenu;         // main menu scene name
     
    public GameObject endLevelCanvas; // instance of the pausedMenuCanvas

    // Initialization
    void Start()
    {
        currentLevel = Application.loadedLevelName; // stores current scene's name
        endLevelCanvas.SetActive(false);    // hides the pause menu canvas
        Time.timeScale = 1f;                // reverts time
    }
    
    // Handles restart button functionality
    public void Restart()
    {
        Application.LoadLevel(currentLevel); // loads current level scene
    }

    // Handles level select button functionality
    public void LevelSelect()
    {
        Application.LoadLevel(levelSelect); // loads level select screen
    }

    // Handles quit button functionaliy
    public void Quit()
    {
        Application.LoadLevel(mainMenu); // loads MainMenu Scene
    }

    // Displays the EndLevelMenu canvas
    public void ShowEndLevelMenu()
    {
        endLevelCanvas.SetActive(true);  // hides the pause menu canvas
        Time.timeScale = 0f;             // reverts time
    }
}