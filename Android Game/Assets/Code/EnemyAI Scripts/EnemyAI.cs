using UnityEngine;

/*
 * This class is the collection of all enemy AIs. Besides movement and projectile firing,
 * this class also handles its respawn and instantiating any items that it can drop.
 * This allows the player to be rewarded with bonuses such as new weapons or
 * recovery items.
 */
public class EnemyAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{

    /* Beginning of All Enemy Type Parameters */
    public float MovementSpeed;         // travel speed of this GameObject
    public GameObject DestroyedEffect;  // the destroyed effect
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    public int MaxHealth = 100;                         // maximum health of the this GameObject
    public int CurrentHealth { get; private set; }      // this GameObject's current health    

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public GameObject[] ItemDroplist;           // array of items that the enemy can drop
    public SpriteRenderer SpriteColor;          // reference to the AI's sprite color
    public bool CanFireProjectiles;             // used by AllProjectiles to disable AI from firing projectiles
    /* End of All Enemy Type Parameters */

    /* Enemies with Projectiles */
    public float FireRate = 1;                  // cooldown time after firing a projectile
    public float Cooldown;                      // the amount of time this GameObject can shoot projectiles
    public Projectile Projectile;               // this GameObject's projectile
    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    public AudioClip ShootSound;                // the sound when this GameObject shoots a projectile
   
    /* Enemies using OverlapCircle */
    private Player Player;                  // instance of the player class
    public float PlayerDetectionRadius;     // the distance between the Player Object and this GameObject
    private bool IsPlayerInRange;           // used to determine if the Player Object is in range of this GameObject
    private bool IsPlayerFacingAway;        // if the Player Object is not facing this GameObject
    public LayerMask DetectThisLayer;       // determines what this GameObject is colliding with

    /* Guardian */
    public GameObject ProjectileSpawnEffect;    // effect played when spawning the projectiles

    /* PathedProjectileSpawner */
    public Transform Destination;           // the location where the projectile will travel to
    public float ProjectileSpeed;           // the travel speed of the projectile towards its destination
    public Animator anim;                   // animation

    /* SelfDestruct*/
    public GameObject BlowupEffect;     // the blowup effect
    public AudioClip BlowupSound;       // sound played when this GameObject collides with the Player

    /* Shielder */
    public GameObject Shield;           // collider where special effects of the shield are activated
    public GameObject BlockedEffect;    // effect played when the shield blocks a projectile
    public AudioClip BlockedSound;      // sound played when the shield blocks a projectile

    /* EnemySpawner */
    public GameObject SpawnedEnemy;     // the enemy prefab to be spawned
    public float spawnTime = 3f;        // how long between each spawn
    public Transform[] spawnPoints;     // an array of the spawn points this enemy can spawn from              

    /* DeathSpawn */
    private Vector3 _currentPosition;   // current position of the AI
    public GameObject EnemyPrefab;      // the enemy prefab to spawn
    public GameObject SpawnEffect;      // the effect played when the enemy is spawned

    /* Stalker */
    private float StoredSpeed;          // stores original movement speed

    public enum EnemyType               // enemy behavior based on type
    {
        Charger,                        // movement speed is increased upon raycast returning true
        Guardian,                       // will fire projectiles when Physics2D.overlapcircle returns true
        Jumper,                         // jumps and patrols an area
        Patrol,                         // moves back and forth between an area, changing direction upon collision with platforms
        PatrolShoot,                    // patrols an area and fires projectiles with raycast
        PatrolTurn,                     // 
        PathedProjectileSpawner,        // spawns a projectile that travels torward a set 'destination'
        SelfDestruct,                   // destroys itself upon collision with the player
        Stalker,                        //
        EnemySpawner,                   // periodically spawns enemy based on a transform.position
        DeathSpawn                      // spawns an enemy upon death

    }
    public EnemyType Enemy;             // instance of an EnemyType, used to determine AI behavior
    
    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        CurrentHealth = MaxHealth;                              // sets current health to maximum health
        Player = FindObjectOfType<Player>();                    // finds instances of the player
        SpriteColor.color = Color.white;                        // sets the color to white by default
        CanFireProjectiles = true;                              // by default allows AI to shoot projectiles
        StoredSpeed = MovementSpeed;                            // stores original movement speed

        if (Enemy == EnemyType.PathedProjectileSpawner)         // sets the PathedProjectileSpawner cooldown
            Cooldown = FireRate;

        if(Enemy == EnemyType.EnemySpawner)
        {
            // calls the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            InvokeRepeating("Spawn", spawnTime, spawnTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check to see if the player is facing away from the AI
        if ((Player.transform.position.x < transform.position.x && Player.transform.localScale.x < 0)
        || (Player.transform.position.x > transform.position.x && Player.transform.localScale.x > 0))
        {
            IsPlayerFacingAway = true;
        }
        else
            IsPlayerFacingAway = false;

        // Handles basic movement
        if (Enemy != EnemyType.PathedProjectileSpawner)
        {
            // Sets the x-velocity of this GameObject
            _controller.SetHorizontalForce(_direction.x * MovementSpeed);

            // Checks to see if this GameObject is colliding with something in the same direction
            if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
                Reverse();
        }

        /* AI with Projectiles */
        if (Enemy == EnemyType.PatrolShoot || Enemy == EnemyType.PathedProjectileSpawner)
        {
            // Handles when this AI cannot shoot
            if ((Cooldown -= Time.deltaTime) > 0)
                return;

            if (Enemy == EnemyType.PatrolShoot)
            {
                // Casts rays to detect player
                var raycast = Physics2D.Raycast(transform.position, _direction, 10, 1 << LayerMask.NameToLayer("Player"));
                if (!raycast)
                    return;
            }

            if (Enemy == EnemyType.PathedProjectileSpawner)
            {
                // Animation
                if (anim != null)
                    anim.SetTrigger("Fire");
            }

            if(CanFireProjectiles == true)
            {
                FireProjectile();
                
                // Handles Sound when the projectile is instantiated by the AI
                if (ShootSound != null)
                    AudioSource.PlayClipAtPoint(ShootSound, transform.position);
            }
            
            Cooldown = FireRate;    // resets the cooldown
        }

        // Stalker AI
        if (Enemy == EnemyType.Stalker)
        {
            if (IsPlayerFacingAway)
            {
                MovementSpeed = StoredSpeed;
                SpriteColor.color = Color.white;    // makes AI visible, and moves the AI
            }

            if (!IsPlayerFacingAway)                // Player facing at the AI
            {
                MovementSpeed = 0;
                SpriteColor.color = Color.clear;    // make AI invisible
            }

        }

        // START OF PHYSICS2D OVERLAPCIRCLE ENEMIES
        if (Enemy == EnemyType.PatrolTurn || Enemy == EnemyType.Guardian || Enemy == EnemyType.Stalker)
        {
            // Variable used to determine if the DetectThisLayer overlaps with the Circle
            IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);

            // PatrolTurn enemies will turn around if the Player is behind them [DOES NOT WORK]
            if (Enemy == EnemyType.PatrolTurn)
            {
                /*
                // Change direction
                if (IsPlayerInRange && IsPlayerFacingAway)
                {
                    _direction = -_direction; // switches direction
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }*/

                // if player.transform.postion.x > transform.position || player.transform.position.x < x
                // change direction
            }

            // Gaurdian AI
            if (Enemy == EnemyType.Guardian)
            {
                // Handles the event that the Player is in range of dectiong by the AI
                if (IsPlayerInRange)
                {
                    // Check to see when projectiles can be fired
                    if ((Cooldown -= Time.deltaTime) > 0)
                        return;

                    FireProjectile();
                    Cooldown = FireRate;

                    // Handles Sound when the projectile is instantiated
                    if (ShootSound != null)
                        AudioSource.PlayClipAtPoint(ShootSound, transform.position);

                    // Handles Effects when the projectile is instantiated
                    if (ProjectileSpawnEffect != null)
                        Instantiate(ProjectileSpawnEffect, ProjectileFireLocation.transform.position, ProjectileFireLocation.transform.rotation);
                }
            }

        } // END OF PHYSICS2D OVERLAPCIRCLE ENEMIES

        // Jump AI
        if (Enemy == EnemyType.Jumper)
        {
            // Checks to see when the AI can jump
            if (_controller.CanJump)
                _controller.Jump();
        }

        // Charger AI
        if (Enemy == EnemyType.Charger)
        {
            // Casts rays to detect player
            var raycast = Physics2D.Raycast(transform.position, _direction, 10, 1 << LayerMask.NameToLayer("Player"));
            if (!raycast)
            {
                SpriteColor.color = Color.white;
                return;
            }

            // Changes sprite color
            SpriteColor.color = Color.red;

            // Increases the AI speed upon detecting the player
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);
        }

        // DeathSpawn AI
        if (Enemy == EnemyType.DeathSpawn)
            _currentPosition = transform.position;  // constantsly updates current position used to spawn an enemy during death

    } // END OF UPDATE

    // Function that indicates that displays range of the PlayerDetectionRadius
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, PlayerDetectionRadius);

        if(Enemy == EnemyType.PathedProjectileSpawner)
        {
            if (Destination == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Destination.position);
        }
    }

    /*
    * @param other, the other GameObject colliding with this AI
    * Function that handles what happens on collision.
    */
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (Enemy == EnemyType.SelfDestruct)
        {
            if (other.GetComponent<Player>() == null)
                return;

            AudioSource.PlayClipAtPoint(BlowupSound, transform.position);
            Instantiate(BlowupEffect, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    // Function called by EnemySpawner AIs that spawns a enemy at set locations
    public void Spawn()
    {
        // If the player has no health left...
        if (Player.Health <= 0 || CurrentHealth == 0)
            return; // ... exit the function.

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(SpawnedEnemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }

    // Function called by AI to instantiate a projectile and fire it in its direction
    public void FireProjectile()
    {
        // Instantiates the projectile, and initilializes the speed, and direction of the projectile
        var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        projectile.Initialize(gameObject, _direction, _controller.Velocity);
    }

    // Function called by Shield.cs to reflect a projectile
    public void ReflectProjectile()
    {
        // Handles Sound when a projectile is blocked
        if (BlockedSound != null)
            AudioSource.PlayClipAtPoint(BlockedSound, transform.position);

        Instantiate(BlockedEffect, Shield.transform.position, Shield.transform.rotation);
        FireProjectile();
    }

    // Function to change direction and velocity
    public void Reverse()
    {
        // switches direction and flips the sprite
        _direction = -_direction; 
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    /*
    * @param damage, the damage this AI receives
    * @param instigator, the GameObject inflicting damage on this AI
    * Handles how this AI receives damage from the Player
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
        CurrentHealth -= damage;                               // decrement this GameObject's CurrentHealth

        // If this GameObject's CurrentHealth reaches zero
        if (CurrentHealth <= 0)
        {
            // Handles what happens when AI DeathSpawn dies
            if (Enemy == EnemyType.DeathSpawn)
            {
                GameObject clone = Instantiate(EnemyPrefab, _currentPosition, transform.rotation) as GameObject;
                Instantiate(SpawnEffect, transform.position, transform.rotation);
            }

            // Sound and Item drops
            AudioSource.PlayClipAtPoint(EnemyDestroySounds[Random.Range(0, EnemyDestroySounds.Length)], transform.position);
            Instantiate(ItemDroplist[Random.Range(0, ItemDroplist.Length)], transform.position, Quaternion.identity);

            // Death of this AI
            CurrentHealth = 0;
            gameObject.SetActive(false);
        }
    }

    /*
    * @param checkpoint, the last checkpoint the Player has acquired
    * @param player, the Player
    * Function used to respawn this AI after the player respawns at the given checkpoint
    */
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        _direction = new Vector2(-1, 0);                // the direction set to left
        transform.localScale = new Vector3(1, 1, 1);    // resets sprite
        gameObject.SetActive(true);                     // shows this AI
        CurrentHealth = MaxHealth;                      // Resets CurrentHealth
    }
}
