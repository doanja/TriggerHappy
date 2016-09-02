using UnityEngine;

/*
* Resources: Adapted from PathedProjectileSpawner, SimpleFlyingEnemyAI, Player, and PathedProjectileSpawner.
*
* This GameObject is a stand still GameObject with a circle used to detect the Player. If the Player is
* detected, this GameObject will spawn a projectile that will follow the Player. The turret has a set 
* amount of health, which can be decremented by the Player's projectiles. 
*/
public class GuardAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    // Parameters
    public float ProjectileTravelSpeed;     // the travel speed of the projectile towards its destination
    public float FireRate;                  // the rate of shots the projectile will be fired at
    private float Cooldown;                 // the cooldown before firing another shot
    public Transform ProjectileDestination; // the location where the projectile will travel to
    public Transform ProjectileFireLocation;
    public PathedProjectile Projectile;     // the projectile shot
    public GameObject ProjectileSpawnEffect;// effect played when spawning the projectile
    public GameObject DestroyedEffect;      // the destroyed effect
    public int PointsToGivePlayer;          // points awarded to the player upon killing this GameObject      

    // Character Parameters
    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _startPosition;     // the initial spawn position of this GameObject

    // Player Checks
    private Player player;          // instance of the player class
    public LayerMask CollisionMask; // determines what this GameObject is colliding with
    public float PlayerRange;       // the distance between the Player Object and this GameObject
    public bool playerInRange;      // used to determine if the Player Object is in range of this GameObject

    // Health
    public int MaxHealth = 100;                     // maximum health of the this GameObject
    public int Health { get; private set; }         // this GameObject's current health    

    // Sound
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed
    public AudioClip SpawnProjectileSound;  // the sound of the projectile spawning

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        _startPosition = transform.position;
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Spawner code
        if ((Cooldown -= Time.deltaTime) > 0)
            return;

        Cooldown = FireRate;

        // Variable used to determines if the CollisionMask overlaps with the Circle
        playerInRange = Physics2D.OverlapCircle(transform.position, PlayerRange, CollisionMask);

        // If the Player Object is in range of this GameObject, and they are facing away, move this GameObject towards the PlayerObject
        if (playerInRange)
        {
            // Projectile
            var projectile = (PathedProjectile)Instantiate(Projectile, ProjectileFireLocation.transform.position, ProjectileFireLocation.transform.rotation);
            projectile.Initialize(ProjectileDestination, ProjectileTravelSpeed);

            // Handles projectile effects
            if (ProjectileSpawnEffect != null)
                Instantiate(ProjectileSpawnEffect, ProjectileFireLocation.transform.position, ProjectileFireLocation.transform.rotation);

            // Sound
            if (SpawnProjectileSound != null)
                AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);
        }
    }

    // Visual indicator for line of travel for the projectile & PlayerRange
    public void OnDrawGizmos()
    {
        if (ProjectileDestination == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, ProjectileDestination.position);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, PlayerRange);
    }

    /*
     * @param damage, the damage this GameObject receives
     * @param instigator, the GameObject inflicting damage on this GameObject
     * Handles how this GameObject receives damage from the Player Object's projectiles
     */
    public void TakeDamage(int damage, GameObject instigator)
    {
        if (PointsToGivePlayer != 0)
        {
            var projectile = instigator.GetComponent<Projectile>();
            if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
            {
                // Handles points
                GameManager.Instance.AddPoints(PointsToGivePlayer);

                // Handles floating text
                FloatingText.Show(string.Format("+{0}!", PointsToGivePlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
            }
        }

        // Effect played upon the death of this GameObject
        Instantiate(DestroyedEffect, transform.position, transform.rotation);
        Health -= damage;                               // decrement this GameObject's health

        // If this GameObject's health reaches zero
        if (Health <= 0)
        {
            AudioSource.PlayClipAtPoint(EnemyDestroySound, transform.position);
            Health = 0;                                 // sets this GameObject's health to 0 
            gameObject.SetActive(false);                // hides this GameObject
        }
    }

    /*
    * @param checkpoint, the last checkpoint the Player Object has acquired
    * @param player, the Player Object
    * Method used to respawn this GameObject after the player respawns at the given checkpoint
    */
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);                     // shows this GameObject
        Health = MaxHealth;                             // sets current health to the GameObject's max health
    }
}
