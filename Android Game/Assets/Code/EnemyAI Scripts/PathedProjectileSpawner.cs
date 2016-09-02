using UnityEngine;

/*
* Resource:
*
* The class will allow GameObjects to spawn Projectile Objects towards a given destination.
* The number of projectiles fired is dependent on the FireRate and the Cooldown.
* The desintation can be set to anything, and the projectiles will travel until it reached
* its destination. Once it reaches its destination, it will destroy the projectile and fire
* more projectiles again.
*/
public class PathedProjectileSpawner : MonoBehaviour {

    public Transform Destination;           // the location where the projectile will travel to
    public PathedProjectile Projectile;     // the projectile shot
    public GameObject ProjectileSpawnEffect;          // effect played when spawning the projectile

    public float Speed;                     // the travel speed of the projectile towards its destination
    public float FireRate;                  // the rate of shots the projectile will be fired at
    public AudioClip SpawnProjectileSound;  // the sound of the projectile spawning

    private float Cooldown;                 // the cooldown before firing another shot

    public Animator anim;                   // animation

    // Use this for initialization
    void Start () {
        Cooldown = FireRate;
	}
	
	// Update is called once per frame
	void Update () {

        // Spawner code
        if ((Cooldown -= Time.deltaTime) > 0)
            return;

        Cooldown = FireRate;
        var projectile = (PathedProjectile)Instantiate(Projectile, transform.position, transform.rotation); // initializes the projectile
        projectile.Initialize(Destination, Speed); // moving the projectile

        // Handles projectile effects
        if (ProjectileSpawnEffect != null)
            Instantiate(ProjectileSpawnEffect, transform.position, transform.rotation);

        // Sound
        if (SpawnProjectileSound != null)
            AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);

        if (anim != null)
            anim.SetTrigger("Fire");
	}

    // Visual indicator for line of travel for the projectile
    public void OnDrawGizmos()
    {
        if (Destination == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, Destination.position);
    }
}
