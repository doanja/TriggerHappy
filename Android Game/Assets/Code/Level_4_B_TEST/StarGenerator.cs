using UnityEngine;
using System.Collections;

public class StarGenerator : MonoBehaviour {

    public GameObject BackgroundStars;
    public int MaxStars;

    private Color[] starColors = {
            new Color(0.5f, 0.5f, 1f),
            new Color(0f, 1f, 1f),
            new Color(1f, 1f, 0),
            new Color(1f, 0f, 0f)
        };

    // Use this for initialization
    void Start () {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        for(int i = 0; i < MaxStars; ++i)
        {
            GameObject star = (GameObject)Instantiate(BackgroundStars);

            // set star color
            star.GetComponent<SpriteRenderer>().color = starColors[Random.Range(0,4)];

            // set position of the star (randomized x,y coordinate)
            star.transform.position = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

            // randomize speed of the star
            star.GetComponent<BackgroundStars>().Speed = -(1f * Random.value + 0.5f);

            star.transform.parent = transform;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
