using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

    public float MovementSpeed;
    public PlayerBullet[] PlayerProjectile;             // change to allprojectiles
    public GameObject[] ProjectileFireLocation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // Handles Prjectiles
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
        {
           
            for(int i = 0; i < ProjectileFireLocation.Length; i++)
            {
                PlayerBullet projectile = Instantiate(PlayerProjectile[Random.Range(0, PlayerProjectile.Length)]);
                projectile.transform.position = ProjectileFireLocation[i].transform.position; // set bllet initial position
            }
            


        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(x, y).normalized;

        Move(direction);
    }

    void Move(Vector2 direction)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        max.x = max.x - 0.225f;
        min.x = min.x - 0.255f;

        max.y = max.y - 0.285f;
        min.y = min.y - 0.285f;

        Vector2 pos = transform.position;

        pos += direction * MovementSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        transform.position = pos;
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
