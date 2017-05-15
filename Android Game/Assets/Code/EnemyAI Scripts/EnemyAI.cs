using System.Collections;
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
    public float MovementSpeed;                 // travel speed of this GameObject
    public GameObject DestroyedEffect;          // the destroyed effect
    public int PointsToGivePlayer;              // points awarded to the player upon killing this GameObject

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private float StoredSpeed;                  // stores original movement
    private Vector3 _currentPosition;           // current position of the AI
    private float MaxSpeedStore;                // stores the Player's MaxSpeed

    public int MaxHealth = 100;                         // maximum health of the this GameObject
    public int CurrentHealth { get; private set; }      // this GameObject's current health    

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public GameObject[] ItemDroplist;           // array of items that the enemy can drop
    public SpriteRenderer SpriteColor;          // reference to the AI's sprite color
    public bool CanFireProjectiles;             // used by AllProjectiles to disable AI from firing projectiles
    /* End of All Enemy Type Parameters */

    /* Enemies with Projectiles */
    public float MaxProjectileCD = 1;           // time needed to be able to fire projectiles again
    public float Cooldown;                      // current time before being able to fire projectiles
    public Projectile Projectile;               // this GameObject's projectile
    public Transform[] ProjectileFireLocation;  // the location of which the projectile is fired at
    public AudioClip ShootSound;                // the sound when this GameObject shoots a projectile
   
    /* Enemies using OverlapCircle */
    private Player Player;                  // instance of the player class
    public float PlayerDetectionRadius;     // the distance between the Player Object and this GameObject
    private bool IsPlayerInRange;           // used to determine if the Player Object is in range of this GameObject
    private bool IsPlayerFacingAway;        // if the Player Object is not facing this GameObject

    /* Guardian */
    public GameObject GameObjectSpawnEffect;    // effect played when spawning the projectiles

    /* SelfDestruct*/
    public GameObject BlowupEffect;     // the blowup effect
    public AudioClip BlowupSound;       // sound played when this GameObject collides with the Player

    /* Shielder */
    public GameObject Shield;           // collider where special effects of the shield are activated
    public GameObject BlockedEffect;    // effect played when the shield blocks a projectile
    public AudioClip BlockedSound;      // sound played when the shield blocks a projectile

    /* EnemySpawner */
    public GameObject SpawnedEnemy;     // the enemy prefab to be spawned
    public float SpawnTime = 3f;        // how long between each spawn
    public Transform[] SpawnPoints;     // an array of the spawn points this enemy can spawn from              

    /* Deathrattle */
    public GameObject[] EnemyPrefab;    // the enemy prefab to spawn
    public GameObject SpawnEffect;      // the effect played when the enemy is spawned   

    /* Undead */
    private float RevivalTime = 6;                       // time before this EnemyAI is active again
    public Sprite UndeadSprite;                          // sprite shown when this EnemyAI is temporary disabled
    public BoxCollider2D UndeadBoxCollider;              // this Undead EnemyAI's box collider
    public GiveDamageToPlayer UndeadGiveDamageToPlayer;  // this Undead EnemyAI's damage given to the player
    public Animator UndeadAnimator;                      // this Undead EnemyAI's animator
    private Sprite UndeadStoredSprite;                   // this Undead EnemyAI's original sprite              

    /* Summoner */
    public AudioClip SummonedSound;     // sound clip played when Summoner EnemyAI instantiates a GameObject

    /* Elizabeth Mate Buoy */
    private float MaxJumpCD = 0.85f;      // max countdown before a jump can be preformed
    private float CurrentJumpCD;          // used to countdown the time before a jump can be preformed

    public enum EnemyType               // enemy behavior based on type
    {
        Charger,                        // movement speed is increased upon raycast returning true
        Guardian,                       // uses a sphere to detect the Player, can shoot projectiles
        Jumper,                         // jumps and patrols an area
        Patrol,                         // patrols an area, changing direction upon collision with platforms
        PatrolShoot,                    // patrols an area and fires projectiles using raycast
        SelfDestruct,                   // destroys itself upon collision with the player
        Stalker,                        // visible and moves only when the player is facing away
        Spawner,                        // periodically spawns enemy based on set position(s)
        Deathrattle,                    // spawns an EnemyAI prefab upon death
        Pooper,                         // patrols an area, and spawns SelfDestruct AIs
        Undead,                         // revives self after RevivalTime = 0
        Ghost,                          // move torwards the player if the player is in range
        Summoner,                       // summons a prefab using sphere detection
        Ninja                           // call teleports() when Player's projectiles collide with it
    }
    public EnemyType Enemy;             // instance of an EnemyType, used to determine AI behavior

    // Status Handlers for the Player
    public bool CanFreeze;              // slows player's MaxSpeed
    public bool CanConfuse;             // reverses player's direction
    public bool CanPoison;              // damages the player over time
    public bool CanParalyze;            // immobilizes the player

    // Status Handlers
    public enum EnemyStatus
    {
        Normal,
        Frozen,
        Confused,
        Poisoned,
        Paraylyzed
    }
    public EnemyStatus Status;          // the EnemyStatus
    public float DebuffCD;           // max time before debuffs wear off

    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        CurrentHealth = MaxHealth;                              // sets current health to maximum health
        Player = FindObjectOfType<Player>();                    // finds instances of the player
        SpriteColor.color = Color.white;                        // sets the color to white by default
        CanFireProjectiles = true;                              // by default allows AI to shoot projectiles
        StoredSpeed = MovementSpeed;                            // stores original movement speed
        transform.localScale = new Vector2(0.75f, 0.75f);       // fixes resizing issue with touch screen overlay
        MaxSpeedStore = MovementSpeed;                          // stores the Enemy's starting MaxSpeed
        Status = EnemyStatus.Normal;                            // Player will start with Normal Status
        SpriteColor.color = Color.white;                        // sets the color to white by default
        DebuffCD = 2f;

        if (Enemy == EnemyType.Spawner)
        {
            // calls the Spawn function after a delay of the SpawnTime and then continue to call after the same amount of time.
            InvokeRepeating("Spawn", SpawnTime, SpawnTime);
        }

        // Stores the Undead EnemyAI's original sprite
        if (Enemy == EnemyType.Undead)
            UndeadStoredSprite = SpriteColor.sprite;
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

        // Variable used to determine if the Player overlaps with the Circle
        IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, 1 << LayerMask.NameToLayer("Player"));

        // Ghost AI
        if (Enemy == EnemyType.Ghost)
        {
            // Handles movement of this GameObject
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);
            return;
        }

        // Sets the x-velocity of this GameObject
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            Reverse();

        /* AI with Projectiles */
        if (Enemy == EnemyType.PatrolShoot || Enemy == EnemyType.Ninja)
        {
            // Handles when this AI cannot shoot
            if ((Cooldown -= Time.deltaTime) > 0)
                return;

            if (Enemy == EnemyType.PatrolShoot || Enemy == EnemyType.Ninja)
            {
                // Casts rays to detect player
                var raycast = Physics2D.Raycast(transform.position, _direction, 15, 1 << LayerMask.NameToLayer("Player"));
                if (!raycast)
                    return;
            }
            
            if(CanFireProjectiles == true)
                FireProjectile();

            // Resets cooldown
            Cooldown = MaxProjectileCD;
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
        if (Enemy == EnemyType.Guardian || Enemy == EnemyType.Pooper || Enemy == EnemyType.Summoner)
        {
            // AI that uses overlap circle physics
            if (Enemy == EnemyType.Guardian || Enemy == EnemyType.Pooper || Enemy == EnemyType.Summoner)
            {
                // Handles the event that the Player is in range of dectiong by the AI
                if (IsPlayerInRange)
                {
                    // Ghost AI
                    if (Enemy == EnemyType.Ghost)
                    {
                        // Handles movement of this GameObject
                        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);
                        return;
                    }

                    // Check to see when projectiles can be fired
                    if ((Cooldown -= Time.deltaTime) > 0)
                        return;

                    if (Enemy == EnemyType.Guardian)
                    {
                        FireProjectile();

                        // Handles effect for multiple projectile firing locations
                        for (int i = 0; i < ProjectileFireLocation.Length; i++)
                            PlayGameObjectSpawnEffect(GameObjectSpawnEffect, ProjectileFireLocation[i]);
                    }

                    // Pooper AI
                    if (Enemy == EnemyType.Pooper)
                    {
                        // Spawns Poop
                        Instantiate(SpawnedEnemy, transform.position, transform.rotation);
                    }

                    if (Enemy == EnemyType.Summoner)
                    {
                        for (int i = 0; i < ProjectileFireLocation.Length; i++)
                        {
                            Instantiate(SpawnedEnemy, ProjectileFireLocation[i].transform.position, ProjectileFireLocation[i].transform.rotation);
                            PlayGameObjectSpawnEffect(GameObjectSpawnEffect, ProjectileFireLocation[i]);
                            PlaySoundEffect(SummonedSound, transform.position);
                        }
                    }

                    // Resets cooldown
                    Cooldown = MaxProjectileCD;
                }
            }

        } // END OF PHYSICS2D OVERLAPCIRCLE ENEMIES

        // Jump AI
        if (Enemy == EnemyType.Jumper)
        {
            if ((CurrentJumpCD -= Time.deltaTime) > 0)
                return;

            StartCoroutine(CountdownJump());    // starts countdown before being able to jump
            CurrentJumpCD = MaxJumpCD;          // resets the cooldown
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

        // Deathrattle AI
        if (Enemy == EnemyType.Deathrattle)
            _currentPosition = transform.position;  // constantsly updates current position used to spawn an enemy during death

        // Undead AI
        if (Enemy == EnemyType.Undead)
        {
            // Check to see when projectiles can be fired
            if ((Cooldown -= Time.deltaTime) > 0)
                return;

            if (CurrentHealth == 0)
            {
                // Resets AIs fields
                MovementSpeed = StoredSpeed;
                SpriteColor.sprite = UndeadStoredSprite;
                CurrentHealth = MaxHealth;
                UndeadBoxCollider.enabled = true;
                UndeadGiveDamageToPlayer.enabled = true;
                UndeadAnimator.enabled = true;
            }
        }
    } // END OF UPDATE

    // Function displays the range of detection for the AI's Sphere/Raycast
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, PlayerDetectionRadius);
    }

    /*
    * @param other, the other GameObject colliding with this AI
    * Function that handles what happens on collision.
    */
    public void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.GetComponent<Player>() == null)
            return;

        else if (CanFreeze == true)
        {
            Player.Status = Player.PlayerStatus.Frozen;
            Player.MaxSpeed = 3;
            Player.StartCoroutine(Player.CountdownDebuff());  // starts countdown before returning to normal status
            Player.SpriteColor.color = Color.cyan;
        }

        else if (CanConfuse == true)
        {
            Player.Status = Player.PlayerStatus.Confused;
            Player.transform.localScale = new Vector2(-0.5f, -0.5f);
            Player.StartCoroutine(Player.CountdownDebuff());  // starts countdown before returning to normal status
            Player.SpriteColor.color = Color.red;
        }

        else if (CanPoison == true)
        {
            Player.Status = Player.PlayerStatus.Poisoned;
            Player.StartCoroutine(Player.CountdownDebuff());  // starts countdown before returning to normal status
            Player.SpriteColor.color = Color.green;
        }
            
        else if (CanParalyze == true)
        {
            Player.Status = Player.PlayerStatus.Paraylyzed;
            Player.MaxSpeed = 0;
            Player.StartCoroutine(Player.CountdownDebuff());  // starts countdown before returning to normal status
            Player.SpriteColor.color = Color.yellow;
        }
            
        if (Enemy == EnemyType.SelfDestruct || Enemy == EnemyType.Ghost)
        {
            if (Enemy == EnemyType.SelfDestruct)
                PlaySoundEffect(SummonedSound, transform.position);

            PlaySoundEffect(BlowupSound, transform.position);
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
        int spawnPointIndex = Random.Range(0, SpawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(SpawnedEnemy, SpawnPoints[spawnPointIndex].position, SpawnPoints[spawnPointIndex].rotation);
    }

    // Function to countdown before the BossAI can make a call to SummonHelper()
    IEnumerator CountdownJump()
    {
        yield return new WaitForSeconds(MaxJumpCD);
        TimedJump();
        yield return 0;
    }

    // Function to summon an Enemy Prefab at BossAI's current position
    public void TimedJump()
    {
        _controller.Jump();
    }

    // Function called by AI to instantiate a projectile and fire it in its direction
    public void FireProjectile()
    {
        for (int i = 0; i < ProjectileFireLocation.Length; i++)  // handles multiple projectile firing locations
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation[i].position, ProjectileFireLocation[i].rotation);
            projectile.Initialize(gameObject, _direction, _controller.Velocity);

            // Plays ShootSound audio clip when the projectile is instantiated
            PlaySoundEffect(ShootSound, transform.position);
        }
    }

    // Function called by Shield.cs to reflect a projectile
    public void ReflectProjectile()
    {
        PlaySoundEffect(BlockedSound, transform.position);
        Instantiate(BlockedEffect, Shield.transform.position, Shield.transform.rotation);
        FireProjectile();
    }

    // Function called by EnemyAI Ninja
    public void Teleport()
    {
        // positions self above the Player's current position
        transform.position = new Vector2(Player.transform.position.x, transform.position.y + 5);
        PlaySoundEffect(BlockedSound, transform.position);
    }

    // Function to change direction and velocity
    public void Reverse()
    {
        // switches direction and flips the sprite
        _direction = -_direction; 
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Handles Sounds
    public void PlaySoundEffect(AudioClip Sound, Vector3 SoundLocation)
    {
        if (Sound != null)
            AudioSource.PlayClipAtPoint(Sound, SoundLocation);
    }

    // Handles Effects when a GameObject is instantiated
    public void PlayGameObjectSpawnEffect(GameObject Effect, Transform EffectLocation)
    {
        if (Effect != null)
            Instantiate(Effect, EffectLocation.transform.position, EffectLocation.transform.rotation);   
    }

    public IEnumerator CountdownDebuff()
    {
        yield return new WaitForSeconds(DebuffCD);
        MovementSpeed = MaxSpeedStore;
        SpriteColor.color = Color.white;
        Status = EnemyStatus.Normal;

        yield return 0;
    }

    /*
    * @param damage, the damage this AI receives
    * @param instigator, the GameObject inflicting damage on this AI
    * Handles how this AI receives damage from the Player
    */
    public void TakeDamage(int damage, GameObject instigator)
    {
        if (Status == EnemyStatus.Poisoned)
            damage = damage * 2;

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

        if (Enemy == EnemyType.Ninja)
        {
            Teleport();
            Reverse();
            FireProjectile();
        }

        // If this GameObject's CurrentHealth reaches zero
        if (CurrentHealth <= 0)
        {
            // Handles what happens when Deathrattle AI dies
            if (Enemy == EnemyType.Deathrattle)
            {
                Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Length)], _currentPosition, transform.rotation);
                Instantiate(SpawnEffect, transform.position, transform.rotation);
            }

            // Handles what happens when Undead AI dies
            if (Enemy == EnemyType.Undead)
            {
                CurrentHealth = 0;
                SpriteColor.sprite = UndeadSprite;
                MovementSpeed = 0;
                UndeadBoxCollider.enabled = false;
                UndeadGiveDamageToPlayer.enabled = false;
                UndeadAnimator.enabled = false;
                Cooldown = RevivalTime;             // counts down until thing can revive
            }

            if (Enemy != EnemyType.Undead)
            {
                // Sound and Item drops
                AudioSource.PlayClipAtPoint(EnemyDestroySounds[Random.Range(0, EnemyDestroySounds.Length)], transform.position);
                Instantiate(ItemDroplist[Random.Range(0, ItemDroplist.Length)], transform.position, Quaternion.identity);

                // Death of this AI
                CurrentHealth = 0;
                gameObject.SetActive(false);
            }
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
        transform.localScale = new Vector2(0.75f, 0.75f);         // fixes resizing issue with touch screen overlay
    }
}
