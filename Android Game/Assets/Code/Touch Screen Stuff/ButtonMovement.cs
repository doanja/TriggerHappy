using UnityEngine;

public class ButtonMovement : TouchManager {

    public enum type { LeftButton, RightButton, UpButton, DownButton, ShootButton, JumpButton};
    public type buttonType;
    public GUITexture buttonTexture = null;
    private Player thePlayer;

    // Use this for initialization
    void Start () {       
        thePlayer = FindObjectOfType<Player>();
    }
	
	// Update is called once per frame
	void Update () {
        TouchInput(buttonTexture);
	}

    // Do this if jump button is the first button to be pressed
    void OnFirstTouchBegan()
    {
        switch (buttonType)
        {
            case type.JumpButton:
                if (thePlayer._controller.CanJump) {
                   thePlayer._controller.Jump();
                }
                   
                break;
            case type.ShootButton:
                thePlayer.TouchShoot();
                break;
        }
    }

    // Do this if the jump button is the second button to be pressed
    void OnSecondTouchBegan()
    {
        switch (buttonType)
        {
            case type.JumpButton:
                if (thePlayer._controller.CanJump)
                {
                    thePlayer._controller.Jump();
                }
                break;
            case type.ShootButton:
                thePlayer.TouchShoot();
                break;
        }
    }

    // Do this if buttons on the directional buttons are pressed first
    void OnFirstTouch()
    {
        switch (buttonType)
        {
            case type.LeftButton:
                thePlayer.hInput = -1;
                break;
            case type.RightButton:
                thePlayer.hInput = 1;
                break;
            case type.DownButton:
                thePlayer.vInput = -1;
                break;
            case type.UpButton:
                thePlayer.vInput = 1;
                break;
        }
    }

    // Do this if buttons on the directional buttons are pressed second
    void OnSecondTouch()
    {
        switch (buttonType)
        {
            case type.LeftButton:
                thePlayer.hInput = -1;
                break;
            case type.RightButton:
                thePlayer.hInput = 1;
                break;
            case type.DownButton:
                thePlayer.vInput = -1;
                break;
            case type.UpButton:
                thePlayer.vInput = 1;
                break;
        }
    }

    // Do this when directional buttons are not pressed
    void OnFirstTouchEnded()
    {
        thePlayer.hInput = 0;
        thePlayer.vInput = 0;
    }

    // Do this when directional buttons are not pressed
    void OnSecondTouchEnded()
    {
        thePlayer.hInput = 0;
        thePlayer.vInput = 0;
    }
}
