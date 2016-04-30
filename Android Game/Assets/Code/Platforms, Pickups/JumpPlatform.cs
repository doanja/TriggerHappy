using UnityEngine;
using System.Collections;

/* https://www.youtube.com/watch?v=hkLAdo9ODDs&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n&index=12 */

public class JumpPlatform : MonoBehaviour {

    public float JumpMagnitude = 20;
    public AudioClip JumpSound;

    public void ControllerEnter2D(CharacterController2D controller)
    {
        if (JumpSound != null)
            AudioSource.PlayClipAtPoint(JumpSound, transform.position);
        controller.SetVerticalForce(JumpMagnitude);
    }

}
