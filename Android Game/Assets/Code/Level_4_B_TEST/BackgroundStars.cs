using UnityEngine;
using System.Collections;

public class BackgroundStars : MonoBehaviour {

    public float Speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        // current position
        Vector2 position = transform.position;

        // calculate new position
        position = new Vector2(position.x, position.y + Speed * Time.deltaTime);

        // update new position
        transform.position = position;

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if(transform.position.y < min.y)
        {
            transform.position = new Vector2(Random.Range(min.x, max.x), max.y);
        }
    }
}
