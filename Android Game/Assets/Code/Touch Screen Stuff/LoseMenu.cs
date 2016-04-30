using UnityEngine;

/*
* Resource: Adapted from PauseMenu.
*
* Handles functionality of the LoseMenu canvas.
*/
public class LoseMenu : MonoBehaviour {

    // For the buttons
    private string currentLevel;    // current scene name
    public string levelSelect;      // name of the level select screen
    public string mainMenu;         // name of the main menu scene   

    public GameObject loseMenuCanvas;   // instance of the pausedMenuCanvas
    private LifeManager lifeManager;    // instance of the LifeManagerCanvas

    // Initialization
    void Start()
    {
        currentLevel = Application.loadedLevelName; // stores current scene's name
        loseMenuCanvas.SetActive(false);    // hides the pause menu canvas
        Time.timeScale = 1f;                // reverts time

        lifeManager = FindObjectOfType<LifeManager>();
    }    
 
    // Updates once per frame
    void Update()
    {
        // Checks to see how many lives the Player has
        if(lifeManager.lifeCounter == 0)
        {
            ShowLoseMenu(); // displays LoseMenuCanvas
        }
    }

    // Handles restart button functionality
    public void Restart()
    {
        loseMenuCanvas.SetActive(false);        // hides the pause menu canvas
        Time.timeScale = 1f;                    // reverts time
        Application.LoadLevel(currentLevel);    // loads current level scene
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

    // Displays the LoseMenuCanvas
    public void ShowLoseMenu()
    {
        loseMenuCanvas.SetActive(true); // shows the pause menu canvas
        Time.timeScale = 0f;            // freezes time        
    }
}
