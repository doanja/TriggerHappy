using UnityEngine;

/*
* Resource:
*
* This class handles how projectiles are spawned, and their travel distance.
*/
public class PathedProjectile : Projectile, ITakeDamage {

    private Transform _destination;     // the end point of the projectile
    private float _speed;               // the velocity of the projectile
    public float TimeToLive;            // the amount of time this GameObject lives

    public AudioClip DestroySound;      // the sound played when this GameObject dies
    public GameObject DestroyEffect;    // the effect played when this GameObject dies
    public int PointsToGivePlayer;      // amount of points awarded to player

    // Constructor
    public void Initialize(Transform destination, float speed)
    {
        _destination = destination;
        _speed = speed;
    }
	
	// Update is called once per frame
	void Update () {

        // The amount of time this projectile lives
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

        // Handles the travel path of the object
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);
        var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
        if (distanceSquared > 0.1f * 0.01f)
            return;

        // Handles special effects
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        // Sound
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);

        Destroy(gameObject); // destroys the object
	}

    /*
    * @param other, the GameObject entering collision
    * Handles what happens when this object collides with another object    
    */
    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject); // destroys the object
    }

    // Method to destroy the projectile
    private void DestroyProjectile()
    {
        // Handles effects
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        // Handles Sound
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);

        // Destroys this GameObject
        Destroy(gameObject);
    }

    /*
    * @param damage, the damage this GameObject receives
    * @param instigator, the GameObject inflicting damage on this GameObject
    * Handles how this GameObject receives damage from the Player Object's projectiles
    */
    public void TakeDamage(int damage, GameObject instigator)
    {
        // effects
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);

        Destroy(gameObject); // destroys this GameObject

        var projectile = instigator.GetComponent<Projectile>(); // the projectile causing damage to this GameObject

        // if the owner of the Projectile is the Player Object
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null && PointsToGivePlayer != 0)
        {
            // award points to the player
            GameManager.Instance.AddPoints(PointsToGivePlayer);
            FloatingText.Show(string.Format("+{0}!", PointsToGivePlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        }
    }
}