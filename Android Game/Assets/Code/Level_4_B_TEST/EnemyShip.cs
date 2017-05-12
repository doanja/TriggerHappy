using UnityEngine;
using System.Collections;

public class EnemyShip : MonoBehaviour {

    public float MovementSpeed;
    public GameObject ShipProjectile;

    // Use this for initialization
    void Start () {
        Invoke("FireProjectile", 1f);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 position = transform.position;

        position = new Vector2(position.x, position.y - MovementSpeed * Time.deltaTime);

        transform.position = position;

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        if(transform.position.y < min.y)
        {
            Destroy(gameObject);
        }
	}

   private void FireProjectile()
    {
        GameObject playerShip = GameObject.Find("PlayerShip");

        if(playerShip != null)
        {
            GameObject projectileClone = (GameObject)Instantiate(ShipProjectile);

            projectileClone.transform.position = transform.position;

            Vector2 direction = playerShip.transform.position - projectileClone.transform.position;

            projectileClone.GetComponent<ShipProjectiles>().SetDirection(direction);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.GetComponent<PlayerShip>() == null || other.GetComponent<PlayerBullet>() == null)
        //  return;

        if (other.tag == "Player")
            Destroy(gameObject);
        //else

    }
}
