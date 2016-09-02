using UnityEngine;

/*
* Class destroys incoming Projectiles from the Player upon colliding with this GameObject
* and instantiates a projectile back.
*/
public class ReflectProjectiles : MonoBehaviour {  

    // Character Essentials    
    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    // Projectile
    public Projectile Projectile;               // this GameObject's projectile
    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    public GameObject ProjectileSpawnEffect;    // effect played when spawning the projectile

    // Sound
    public AudioClip BlockProjectileSound;            // the sound when this GameObject shoots a projectile

    private EnemyAI Enemy;                  // instance of the player class

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject 

        Enemy = FindObjectOfType<EnemyAI>();
    }
    
    /*
    * @param other, the other GameObject colliding with this GameObject
    * Function that handles what happens on collision.
    */
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Does nothing if other is not a projectile
        if (other.GetComponent<Projectile>() == null)
            return;

        // If other is an instance of a SimpleProjectile
        var projectile = other.GetComponent<SimpleProjectile>();
        
        // Checks to see if the owner of the projectile is the player
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
        {
            // Handles projectile effects
            if (ProjectileSpawnEffect != null)
                Instantiate(ProjectileSpawnEffect, ProjectileFireLocation.transform.position, ProjectileFireLocation.transform.rotation);
        
            // Sound
            if (BlockProjectileSound != null)
                AudioSource.PlayClipAtPoint(BlockProjectileSound, transform.position);

            DestroyObject(other); // destroys the projectile
        }
    }
}
