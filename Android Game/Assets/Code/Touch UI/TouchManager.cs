using UnityEngine;

public class TouchManager : MonoBehaviour {

    public static bool GUITouch = false;

    // Checks if you touched a texture
    public void TouchInput(GUITexture texture)
    {
        if (texture.HitTest(Input.GetTouch(0).position))
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    SendMessage("OnFirstTouchBegan");                    
                    SendMessage("OnFirstTouch");
                    GUITouch = true;
                    break;
                case TouchPhase.Stationary:
                    SendMessage("OnFirstTouchStayed");
                    SendMessage("OnFirstTouch");
                    GUITouch = true;
                    break;
                case TouchPhase.Moved:
                    SendMessage("OnFirstTouchMoved");
                    SendMessage("OnFirstTouch");
                    GUITouch = true;
                    break;
                case TouchPhase.Ended:
                    SendMessage("OnFirstTouchEnded");
                    GUITouch = false;
                    break;
            }
        }
        if (texture.HitTest(Input.GetTouch(1).position))
        {
            switch (Input.GetTouch(1).phase)
            {
                case TouchPhase.Began:
                    SendMessage("OnSecondTouchBegan");
                    SendMessage("OnSecondTouch");
                    break;
                case TouchPhase.Stationary:
                    SendMessage("OnSecondTouchStayed");
                    SendMessage("OnSecondTouch");
                    break;
                case TouchPhase.Moved:
                    SendMessage("OnSecondTouchMoved");
                    SendMessage("OnSecondTouch");
                    break;
                case TouchPhase.Ended:
                    SendMessage("OnSecondTouchEnded");
                    break;
            }
        }
    }
}
