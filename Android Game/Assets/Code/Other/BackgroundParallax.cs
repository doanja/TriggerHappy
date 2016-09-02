using UnityEngine;
using System.Collections;

/* 
* Resource: https://www.youtube.com/watch?v=cpt6MugGloM&index=16&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n 
* This class is used to make 2D background images appear as if they're moving as the player
* move towards to the right of the level.
*/

public class BackgroundParallax : MonoBehaviour {

    public Transform[] Backgrounds;         // array of background images
    public float ParallaxScale;             // demonstrates the Paralax effects
    public float ParallaxReductionFactor;   // make backgrounds images further/slower
    public float Smoothing;

    private Vector3 _lastPosition;          // the last position of the this GameObject

    // Use this for initialization
    public void Start()
    {
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    public void Update()
    {
        var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;
        for(var i = 0; i < Backgrounds.Length; i++)
        {
            var backgroundTargetPosition = Backgrounds[i].position.x + parallax * (i * ParallaxReductionFactor + 1);
            Backgrounds[i].position = Vector3.Lerp(
                Backgrounds[i].position,
                new Vector3(backgroundTargetPosition, Backgrounds[i].position.y, Backgrounds[i].position.z),
                Smoothing * Time.deltaTime);
        }
        _lastPosition = transform.position; // Re-initializes the position of this GameObject
    }
}
