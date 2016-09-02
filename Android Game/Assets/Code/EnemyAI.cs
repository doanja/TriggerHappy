using UnityEngine;

public class EnemyAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{

    /* Beginning of All Enemy Type Parameters*/
    public float MovementSpeed;         // travel speed of this GameObject
    public GameObject DestroyedEffect;  // the destroyed effect
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    public int MaxHealth = 100;                 // maximum health of the this GameObject
    public int Health { get; private set; }     // this GameObject's current health    

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public GameObject[] ItemDroplist;
    /* End of All Enemy Type Parameters*/

    /* Enemies with Projectiles */
    public float FireRate = 1;                  // cooldown time after firing a projectile
    private float Cooldown;                     // the amount of time this GameObject can shoot projectiles
    public Projectile Projectile;               // this GameObject's projectile
    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    public AudioClip ShootSound;                // the sound when this GameObject shoots a projectile
   
    /* Enemies using OverlapCircle */
    private Player Player;                  // instance of the player class
    public float PlayerDetectionRadius;     // the distance between the Player Object and this GameObject
    public bool IsPlayerInRange;            // used to determine if the Player Object is in range of this GameObject
    public bool IsPlayerFacingAway;         // if the Player Object is not facing this GameObject
    public LayerMask DetectThisLayer;       // determines what this GameObject is colliding with

    /* Shielder */
    public GameObject ProjectileSpawnEffect;    // effect played when spawning the projectiles

    /* PathedProjectileSpawner */
    public Transform Destination;           // the location where the projectile will travel to
    public float ProjectileSpeed;           // the travel speed of the projectile towards its destination
    public Animator anim;                   // animation

    /* SelfDestruct*/
    public GameObject BlowupEffect;     // the blowup effect
    public AudioClip BlowupSound;       // sound played when this GameObject collides with the Player

    public enum EnemyType               // enemy behavior based on type
    {
        Charger,
        Jumper,
        Patrol,
        PatrolShoot,
        PatrolTurn,
        PathedProjectileSpawner,
        SelfDestruct,
        Shielder,
        Stalker
    }
    public EnemyType Enemy;             // instance of an EnemyType, used to determine AI behavior
    
    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        Health = MaxHealth;
        Player = FindObjectOfType<Player>();

        if (Enemy == EnemyType.PathedProjectileSpawner)
            Cooldown = FireRate;
    }

    // Update is called once per frame
    void Update()
    { 
        // Handles basic movement
        if(Enemy != EnemyType.PathedProjectileSpawner)
        {
            _controller.SetHorizontalForce(_direction.x * MovementSpeed); // Sets the x-velocity of this GameObject

            // Checks to see if this GameObject is colliding with something in the same direction
            if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            {
                _direction = -_direction; // switches direction
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        /* Projectiles */
        if (Enemy == EnemyType.PatrolShoot || Enemy == EnemyType.PathedProjectileSpawner)
        {
            // Handles when this GameObject cannot shoot
            if ((Cooldown -= Time.deltaTime) > 0)
                return;

            if (Enemy == EnemyType.PatrolShoot)
            {
                // Casts rays to detect player
                var raycast = Physics2D.Raycast(transform.position, _direction, 10, 1 << LayerMask.NameToLayer("Player"));
                if (!raycast)
                    return;

                // Instantiates the projectile, and initilializes the speed, and direction of the projectile
                var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
                projectile.Initialize(gameObject, _direction, _controller.Velocity);

                Cooldown = FireRate; // time frame, when projectiles can be shot from this GameObject

                // Handles Sound
                if (ShootSound != null)
                    AudioSource.PlayClipAtPoint(ShootSound, transform.position);
            }

            if (Enemy == EnemyType.PatrolShoot)
            {
                var projectile = (PathedProjectile)Instantiate(Projectile, transform.position, transform.rotation); // initializes the projectile
                projectile.Initialize(Destination, ProjectileSpeed); // moving the projectile

                // Handles projectile effects
                if (ProjectileSpawnEffect != null)
                    Instantiate(ProjectileSpawnEffect, transform.position, transform.rotation);

                Cooldown = FireRate; // time frame, when projectiles can be shot from this GameObject

                // Sound
                if (ShootSound != null)
                    AudioSource.PlayClipAtPoint(ShootSound, transform.position);

                if (anim != null)
                    anim.SetTrigger("Fire");
            }

            

            
        }

        /* Jump */
        if (Enemy == EnemyType.Jumper)
        {
            // Handles jumping
            if (_controller.CanJump)
                _controller.Jump();
        }

        // untested from GhostAI
        if (Enemy == EnemyType.PatrolTurn || Enemy == EnemyType.Stalker || Enemy == EnemyType.Charger || Enemy == EnemyType.Shielder)
        {
            // Variable used to determine if the DetectThisLayer overlaps with the Circle
            IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);

            // If the Player Object is on the left of this GameObject and is facing IsPlayerFacingAway or vice versa
            if ((Player.transform.position.x < transform.position.x && Player.transform.localScale.x < 0)
            || (Player.transform.position.x > transform.position.x && Player.transform.localScale.x > 0))
            {
                IsPlayerFacingAway = true;
            }
            else
                IsPlayerFacingAway = false;

            // PatrolTurn enemies will turn around if the Player is behind them
            if (Enemy == EnemyType.PatrolTurn)
            {
                // Change direction
                if (IsPlayerInRange && IsPlayerFacingAway)
                {
                    _direction = -_direction; // switches direction
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
            }

            // Stalkers moves only if the Player is facing away
            else if (Enemy == EnemyType.Stalker)
            {
                // If the Player Object is in range of this GameObject, and they are facing IsPlayerFacingIsPlayerFacingAway, move this GameObject towards the PlayerObject
                if (IsPlayerInRange && IsPlayerFacingAway)
                {
                    // Handles movement of this GameObject
                    transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);
                    return;
                }
            }
            
            /* working */
            if (Enemy == EnemyType.Charger)
            {
                // Casts rays to detect player
                var raycast = Physics2D.Raycast(transform.position, _direction, 10, 1 << LayerMask.NameToLayer("Player"));
                if (!raycast)
                    return;

                transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);
            }

            /* WORKING */
            if (Enemy == EnemyType.Shielder)
            {
                // Variable used to determines if the CollisionMask overlaps with the Circle
                IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);

                // If the Player Object is in range of this GameObject, and they are facing away, move this GameObject towards the PlayerObject
                if (IsPlayerInRange)
                {
                    // Handles when this GameObject cannot shoot
                    if ((Cooldown -= Time.deltaTime) > 0)
                        return;

                    // Instantiates the projectile, and initilializes the speed, and direction of the projectile
                    var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
                    projectile.Initialize(gameObject, _direction, _controller.Velocity);
                    Cooldown = FireRate; // time frame, when projectiles can be shot from this GameObject
                        
                    // Handles Sound
                    if (ShootSound != null)
                        AudioSource.PlayClipAtPoint(ShootSound, transform.position);

                    // Handles projectile effects
                    if (ProjectileSpawnEffect != null)
                        Instantiate(ProjectileSpawnEffect, ProjectileFireLocation.transform.position, ProjectileFireLocation.transform.rotation);
                }
            }
        } // END OF PHYSICS2D OVERLAPCIRCLE ENEMIES





        
    } // END OF UPDATE

    // Method draws a sphere indicating the range of view of this GameObject
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
    * @param other, the other GameObject colliding with this GameObject
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
            gameObject.SetActive(false);                // hides this GameObject
        }
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
            AudioSource.PlayClipAtPoint(EnemyDestroySounds[Random.Range(0, EnemyDestroySounds.Length)], transform.position);
            Instantiate(ItemDroplist[Random.Range(0, ItemDroplist.Length)], transform.position, Quaternion.identity);
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
        // Re-initializes this GameObject's direction, and start position
        _direction = new Vector2(-1, 0);                // the direction set to left
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.SetActive(true);                     // shows this GameObject                                      

        // Resets health
        Health = MaxHealth;                             // sets current health to the GameObject's max health
    }
}
