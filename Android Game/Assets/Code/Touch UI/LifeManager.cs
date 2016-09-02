using UnityEngine;
using UnityEngine.UI;

/*
* Resources: https://www.youtube.com/watch?v=zr7ys5lFakA&list=PLiyfvmtjWC_Up8XNvM3OSqgbJoMQgHkVz&index=20
*
* Handles Player's lives.
*/
public class LifeManager : MonoBehaviour {

    public int startingLives;   // initiali number of lives
    public int lifeCounter;     // current number of lives
    private Text theText;       // text field for the LifeManager canvas    

	// Use this for initialization
	void Start () {
        theText = GetComponent<Text>();
        lifeCounter = startingLives;
	}
	
	// Update is called once per frame
	void Update () {
        theText.text = "x " + lifeCounter;
	}

    // Increments lives
    public void GiveLife()
    {
        lifeCounter++;
    }

    // Decrements lives
    public void TakeLife()
    {
        lifeCounter--;
    }
}
