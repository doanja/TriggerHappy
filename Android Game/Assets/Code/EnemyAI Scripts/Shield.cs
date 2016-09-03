using UnityEngine;

/*
 * Function to handles events of blocking a projectile and invoking
 * the ReflectProjectile function within the EnemyAI script.
 */
public class Shield : MonoBehaviour {

    private EnemyAI Enemy;  // instance of the player class

    // Use this for initialization
    void Start()
    {
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

        // If other is an instance of a AllProjectiles
        var projectile = other.GetComponent<Projectile>();

        // Checks to see if the owner of the projectile is the player
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
        {
            DestroyObject(other); // destroys the projectile
            Enemy.ReflectProjectile();
        }
    }
}
