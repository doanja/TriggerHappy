using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

    public float ProjectileSpeed = 8f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // current position
        Vector2 position = transform.position;

        // calculate projectile new position
        position = new Vector2(position.x, position.y + ProjectileSpeed * Time.deltaTime);

        // update projectile's new position
        transform.position = position;

        // top right of the screen
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // if the projectile reaches the end top screen
        if (transform.position.y > max.y)
            Destroy(gameObject);
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.GetComponent<PlayerShip>() == null || other.GetComponent<PlayerBullet>() == null)
        //  return;

        if (other.tag == "Enemies")
            Destroy(gameObject);
        //else

    }
}
