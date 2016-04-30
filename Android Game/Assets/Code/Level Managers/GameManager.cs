using UnityEngine;

/* https://www.youtube.com/watch?v=Za6dBkmXSuQ&index=20&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n
*
* This class handles the Player's accumulated points. 
*/

public class GameManager : MonoBehaviour {

    private static GameManager _instance;
    
    // return _instance if it exists, otherwise, set instance to a new GameManager
    public static GameManager Instance { get { return _instance ?? (_instance = new GameManager()); } }

    // variable to calculate how many points the player has
    public int Points { get; private set; }

    // empty constructor, only the GameManger can create an instance of this
    private GameManager()
    {

    }

    // method to reset the player's points when starting a new game
    public void Reset()
    {
        Points = 0;
    }

    // method to reset the player's points before they died
    public void ResetPoints(int points)
    {
        Points = points;
    }

    // method to add newly acquired points to the points the player already has
    public void AddPoints(int pointsToAdd)
    {
        Points += pointsToAdd;
    }
}
