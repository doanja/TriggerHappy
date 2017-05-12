using UnityEngine;
using System.Collections;

public class ShipProjectiles : MonoBehaviour {

    public float ProjectileSpeed;
    private Vector2 _direction;
    public bool isReady;

    void Awake()
    {
        ProjectileSpeed = 5f;
        isReady = false;
    }

	// Use this for initialization
	void Start () {
	
	}
	
    public void SetDirection(Vector2 direction)
    {
        _direction = direction.normalized;
        isReady = true;
    }

	// Update is called once per frame
	void Update () {
        if (isReady)
        {
            Vector2 position = transform.position;

            position += _direction * ProjectileSpeed * Time.deltaTime;

            transform.position = position;

            Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            if((transform.position.x < min.x) || (transform.position.x > max.x) ||
                (transform.position.y < min.y) || (transform.position.y > max.y))
            {
                Destroy(gameObject);
            }
        }
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.GetComponent<PlayerShip>() == null || other.GetComponent<PlayerBullet>() == null)
          //  return;

        if(other.tag == "Player")
            Destroy(gameObject);
        //else

    }
}
