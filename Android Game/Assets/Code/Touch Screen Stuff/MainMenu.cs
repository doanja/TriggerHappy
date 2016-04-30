using UnityEngine;

/*
* Resouce: https://www.youtube.com/watch?v=oJa0FOrHSuY&index=17&list=PLiyfvmtjWC_Up8XNvM3OSqgbJoMQgHkVz
*
* Class to handle functionality of the MainMenu.
*/
public class MainMenu : MonoBehaviour {

    public string startLevel;   // name of the main menu screen
    public string levelSelect;  // name of the level select screen    

    // Loads the level 1 scene [New Game]
	public void NewGame()
    {
        Application.LoadLevel(startLevel);
        //LevelManager.Instance.GotoNextLevel(startLevel);
    }  

    // Loads the level select screen
    public void LevelSelect()
    {        
        Application.LoadLevel(levelSelect);
    }
}
