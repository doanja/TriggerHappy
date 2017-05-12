using UnityEngine;
using System.Collections;

public class BackgroundPlanet : MonoBehaviour {

    public float Speed;
    public bool isMoving;
    
    Vector2 min;
    Vector2 max;
    
    void Awake()
    {
        
        isMoving = false;

        min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // add the planet sprite half height to max y
        //max.y = max.y + GetComponent<SpriteRenderer>().sprite.bounds.extents.y;

        // subtract the planet sprite half height to min y
        //min.y = min.y + GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
        
    }

	// Use this for initialization
	void Start () {

      
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
            return;

        // current position
        Vector2 position = transform.position;

        // calculate new position
        position = new Vector2(position.x, position.y + Speed * Time.deltaTime);

        // update new position
        transform.position = position;

        //Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        //Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // if the planet gets to the minimum y position, then stop moving the planet
        if (transform.position.y < min.y)
            isMoving = false;
            
	}

    // Function to reset the planet's position
    public void ResetPosition()
    {
        transform.position = new Vector2(Random.Range(min.x, max.x), 1.5f*(max.y + GetComponent<SpriteRenderer>().sprite.bounds.extents.y));
        // need to reset out of camera view
    }
}
