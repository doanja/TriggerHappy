using UnityEngine;

/*
* Resource: www.youtube.com/watch?v=ieXwKpbpGVk&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n&index=26
*
* This class is used by GameObject who shoot projectiles. This Projectile will travel in a similar
* pathing to sine functions which is determined by its frequency and magnitude. The Projectile
* is undestroyable by the Player, but it has a certain time to live.
*/
public class SinProjectile : Projectile
{

    public int Damage;                  // the damage this projectile inflicts
    public GameObject DestroyedEffect;  // the effect played upon the destruction of this GameObject
    public float TimeToLive;            // the amount of time this GameObject lives
    public AudioClip DestroySound;      // the sound played when this GameObject dies

    public float MoveSpeed = 5.0f;
    public float frequency = 10.0f;  // Speed of sine movement
    public float magnitude = 5.0f;   // Size of sine movement
    private Vector3 axis;
    private Vector3 pos;

    void Start()
    {
        pos = transform.position;
        //DestroyObject(gameObject, 1.0f);
        axis = transform.up;  // May or may not be the axis you want

    }

    // Use this for instantiation
    public void Update()
    {
        // The amount of time this projectile lives
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

        // Handles the speed of the projectile
        pos += transform.right * Time.deltaTime * MoveSpeed;

        // Handles the position of the projectile
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
        //transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
    }


    /*
    * @param other, the other GameObject
    * Instance of this projectile is destroyed
    */
    protected override void OnCollideOther(Collider2D other)
    {
        DestroyProjectile();
    }

    /*
    * @param other, the other GameObject
    * @param takeDamage, the amount of damage that the other GameObject receives
    * On collision, the other GameObject takes damage
    */
    protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {
        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile(); // destroys the projectile
    }

    // Method to destroy the projectile
    private void DestroyProjectile()
    {
        // Handles effects
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        // Handles Sound
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);

        // Destroys this GameObject
        Destroy(gameObject);
    }
}
