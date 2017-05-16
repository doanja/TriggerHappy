using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    // Standard AI Variables
    public float MovementSpeed;                 // travel speed of this GameObject
    public GameObject DestroyedEffect;          // the destroyed effect
    public int PointsToGivePlayer;              // points awarded to the player upon killing this GameObject

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Player Player;                      // instance of the player class

    public int MaxHealth = 100;                         // maximum health of the this GameObject
    public int CurrentHealth { get; private set; }      // this GameObject's current health    
    public GameObject HealthBar;                        // health bar to display this AI's current health

    public int MaxArmor = 0;                            // maximum armor of this GameObject
    public int CurrentArmor { get; private set; }       // this GameObject's current armor
    public GameObject ArmorBar;                         // armor bar to display this AI's current armor

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public SpriteRenderer SpriteColor;          // reference to the AI's sprite color
    private Vector3 _currentPosition;           // current position of the AI
    public GameObject gate;

    public bool HalfDamage;                     // damage taken by this BossAI is halved when true
    public bool ImmuneToDamage;                 // damage taken by this BossAI is nulled when true

    // Projectiles
    public float MaxProjectileCD = 4;           // time needed to fire again
    public float CurrentProjectileCD;           // current time before being able to fire projectiles
    public Projectile[] Projectile;             // this BossAI's projectile
    public Transform[] ProjectileFireLocation;  // the location of which the projectile is fired at
    public AudioClip[] ShootSound;              // the sound when this GameObject shoots a projectile
    public GameObject[] GameObjectSpawnEffect;  // effects played when BossAI does something 

    // Helper
    public GameObject[] Helpers;            // array of helpers that can be summoned
    public GameObject SpawnEffect;          // effect shown when the Helper is Spawned
    public Transform[] SpawnPoints;         // locations where Helpers can be spawned
    public float MaxActionCD1;              // max countdown before an action can be preformed
    public float MaxActionCD2;              // max countdown before an action can be preformed
    public float CurrentActionCD1;          // used to countdown the time before an action can be taken by the AI
    public float CurrentActionCD2;          // used to countdown the time before an action can be taken by the AI

    // Slime
    public GameObject HealEffect;           // effect played when this BossAI heals itself

    // Ogre
    public bool RageActive;                 // true when Barrier is active
    public GameObject Barrier;              // EnemyAI that helps BossAI Ogre shoot projectiles
    public GameObject Swamp;                // summons water onto the level

    // Pirate
    public int ShotsFired = 0;

    public enum EnemyType
    {
        Slime,
        Ogre,
        Pirate,
        Vader
    }
    public EnemyType Enemy;

    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        Player = FindObjectOfType<Player>();                    // finds instances of the player
        CurrentHealth = MaxHealth;                              // sets current health to maximum health
        transform.localScale = new Vector2(0.75f, 0.75f);       // fixes resizing issue with touch screen overlay

        if (Enemy == EnemyType.Ogre)
        {
            Barrier.SetActive(false);
            Swamp.SetActive(false);
        }

        if (Enemy == EnemyType.Vader || Enemy == EnemyType.Slime)
        {
            CurrentArmor = 100;
        }
    }
	
	// Update is called once per frame
	void Update () {

        // Handles basic movement
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            Reverse();

        // BossAI Slime
        if (Enemy == EnemyType.Slime)
        {
            // Checks cooldown count
            if ((CurrentActionCD1 -= Time.deltaTime) > 0 && (CurrentActionCD2 -= Time.deltaTime) > 0)
                return;

            StartCoroutine(CountdownRegen());   // starts countdown before regenerating health
            CurrentActionCD1 = MaxActionCD1;    // resets the cooldown

            StartCoroutine(CountdownSummon());  // starts countdown before summoning a helper
            CurrentActionCD2 = MaxActionCD2;    // resets the cooldown
        }
        
        // BossAI Ogre
        if (Enemy == EnemyType.Ogre)
        {
            // Phase 1
            if(CurrentHealth > (MaxHealth * 0.5))
            {
                // Barrier inactive
                if (RageActive == false)
                {
                    // Waiting to summoning the Barrier
                    if ((CurrentActionCD2 -= Time.deltaTime) > 0)
                        return;

                    StartCoroutine(CountdownBarrierDown());
                    CurrentActionCD2 = MaxActionCD2;
                }

                // Barrier active
                if (RageActive == true)
                {
                    // Waiting to kill the Barrier
                    if ((CurrentActionCD1 -= Time.deltaTime) > 0)
                        return;

                    StartCoroutine(CountdownBarrierUp());
                    CurrentActionCD1 = MaxActionCD1;
                }
            }

            else
            {
                // Handles when this AI cannot shoot
                if ((CurrentActionCD2 -= Time.deltaTime) > 0)
                    return;

                Barrier.SetActive(false);
                HalfDamage = true;
                Ogredrive();
                CurrentActionCD2 = MaxActionCD2;
                Swamp.SetActive(true);
            }   
        }

        // BossAI Pirate
        if (Enemy == EnemyType.Pirate)
        {
            PiratingTime();

            // Phase 1
            if (CurrentHealth > 100)
            {
                ImmuneToDamage = true;

                // Waiting to Summon Barrels
                if ((CurrentActionCD2 -= Time.deltaTime) > 0)
                    return;

                StartCoroutine(CountdownSummon());
                CurrentActionCD2 = MaxActionCD2;   
            }

            // Phase 2
            else if(CurrentHealth <= 100)
            {
                ImmuneToDamage = false;
                MovementSpeed = 4;
                CurrentProjectileCD = 0.5f;
            }
        }

        if (Enemy == EnemyType.Vader)
        {
            HalfDamage = true;

            // Phase 1
            if (CurrentHealth > 100)
            {
                // Summons the bubble after shooting 5 sabers
                if (ShotsFired == 5)
                {
                    // Handles when this AI cannot shoot
                    if ((CurrentProjectileCD -= Time.deltaTime) > 0)
                        return;

                    SummonHelper();
                    CurrentProjectileCD = MaxProjectileCD;
                    ShotsFired = 0;
                }

            
                // Handles Saber Projectiles
                if ((CurrentProjectileCD -= Time.deltaTime) > 0 && ShotsFired < 5)
                    return;

                FireProjectile();
                PlayGameObjectSpawnEffect(GameObjectSpawnEffect[0], ProjectileFireLocation[0]);
                ShotsFired++;
                CurrentProjectileCD = MaxProjectileCD;
            }
            
            // Phase 2
            if (CurrentHealth <= 100)
            {
                if ((CurrentActionCD1 -= Time.deltaTime) > 0)
                    return;

                StartCoroutine(CountdownRegen());   // starts countdown before regenerating health
                StartCoroutine(AwakenForceField()); // starts countdown before summoning the force field
                CurrentActionCD1 = MaxActionCD1;
            }
        }


    }

    public void Teleport()
    {
        transform.position = new Vector2(Player.transform.position.x, 10);
    }

    IEnumerator AwakenForceField()
    {
        yield return new WaitForSeconds(MaxActionCD1);
        Instantiate(Projectile[1], transform.position, transform.rotation);
        PlaySoundEffect(ShootSound[2], transform.position);
        yield return 0;
    }

    // Time before Barrier Goes Down
    IEnumerator CountdownBarrierUp()
    {
        yield return new WaitForSeconds(MaxActionCD1);  // kills barrier after MaxActionCD1
        RageActive = false;
        Barrier.SetActive(false);
        yield return 0;
    }

    // Sets Barrier to Active
    IEnumerator CountdownBarrierDown()
    {
        yield return new WaitForSeconds(MaxActionCD2);  // summons a barrier after MaxActionCD2
        RageActive = true;
        Barrier.SetActive(true);
        yield return 0;
    }

    // Function to countdown time before BossAI heals to full health
    IEnumerator CountdownRegen()
    {
        yield return new WaitForSeconds(MaxActionCD1);
        CurrentArmor = MaxArmor;
        Instantiate(GameObjectSpawnEffect[1], transform.position, transform.rotation);
        yield return 0;
    }

    // Function to countdown before the BossAI can make a call to SummonHelper()
    IEnumerator CountdownSummon()
    {
        yield return new WaitForSeconds(MaxActionCD2);
        SummonHelper();
        yield return 0;
    }

    // Function to summon an Enemy Prefab at BossAI's current position
    public void SummonHelper()
    {
        if (Enemy == EnemyType.Vader)
            PlaySoundEffect(ShootSound[1], transform.position);

        Instantiate(Helpers[Random.Range(0, Helpers.Length)], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, SpawnPoints[Random.Range(0, SpawnPoints.Length)].rotation);
        Instantiate(SpawnEffect, transform.position, transform.rotation);
    }

    // Function called by AI to instantiate a projectile and fire it in its direction
    public void FireProjectile()
    {
        for(int i = 0; i < ProjectileFireLocation.Length; i++)  // handles multiple projectile firing locations
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile[0], ProjectileFireLocation[i].position, ProjectileFireLocation[i].rotation);
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
        }

        PlaySoundEffect(ShootSound[0], transform.position);
    }

    // Function called by AI to instantiate a projectile and fire it in its direction
    public void FireSecondaryProjectile()
    {
        for (int i = 0; i < ProjectileFireLocation.Length; i++)  // handles multiple projectile firing locations
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile[1], ProjectileFireLocation[i].position, ProjectileFireLocation[i].rotation);
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
        }
    }

    // Handles BossAI Ogre's Phase 2
    public void Ogredrive()
    {
        // Checks to see when the AI can jump
        if (_controller.CanJump)
        {
            _controller.Jump();
            
            // Spawns Poop
            Instantiate(Helpers[0], transform.position, transform.rotation);
            PlaySoundEffect(ShootSound[0], transform.position);
            Instantiate(GameObjectSpawnEffect[0], transform.position, transform.rotation);
        }  
    }

    // Handles projectiles for BossAI type Pirate
    public void PiratingTime()
    {
        // Handle Torpedoes
        if (ShotsFired == 5)
        {
            // Handles when this AI cannot shoot
            if ((CurrentProjectileCD -= Time.deltaTime) > 0)
                return;

            FireSecondaryProjectile();
            PlayGameObjectSpawnEffect(GameObjectSpawnEffect[0], ProjectileFireLocation[0]);
            CurrentProjectileCD = MaxProjectileCD;
            ShotsFired = 0;
        }

        // Handles Projectiles
        if ((CurrentProjectileCD -= Time.deltaTime) > 0 && ShotsFired < 5)
            return;

        FireProjectile();
        PlayGameObjectSpawnEffect(GameObjectSpawnEffect[0], ProjectileFireLocation[0]);
        ShotsFired++;
        CurrentProjectileCD = MaxProjectileCD;
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

    // Function to change direction and velocity
    public void Reverse()
    {
        // switches direction and flips the sprite
        _direction = -_direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
        if (projectile != null || projectile.Owner.GetComponent<Player>() != null)
        {
            if(HalfDamage == true)
                projectile.Damage /= 2;

            if (ImmuneToDamage == true)
                projectile.Damage = 0;
        }
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
        
        if (Enemy == EnemyType.Vader && CurrentHealth < 75 && CurrentArmor == 0)
        {
            FireProjectile();
            Reverse();
            Teleport();
        }
        

        // Health does not get decreased while BossAI has armor
        if (CurrentArmor > 0) {
            CurrentArmor -= damage;

            if (CurrentArmor < 0)
                CurrentArmor = 0;
        }

        else
        {
            // Effect played upon the death of this GameObject
            Instantiate(DestroyedEffect, transform.position, transform.rotation);
            CurrentHealth -= damage;                               // decrement this GameObject's CurrentHealth
        }

        // If this GameObject's CurrentHealth reaches zero
        if (CurrentHealth <= 0)
        {
            // SLIME ONLY: Summon Slime Helpers upon death
            if (Enemy == EnemyType.Slime)
            {
                for (int i = 0; i < SpawnPoints.Length; i++)
                    Instantiate(Helpers[Random.Range(0, SpawnPoints.Length)], SpawnPoints[Random.Range(0,i)].position, SpawnPoints[Random.Range(0, i)].rotation);
            }

            if (Enemy == EnemyType.Vader)
                Helpers[1].SetActive(false);

            gate.SetActive(true);                       // makes end level portal visible
            HealthBar.SetActive(false);                 // hides the health bar
            ArmorBar.SetActive(false);                  // hides the armor bar


            if (Enemy == EnemyType.Vader)
                AudioSource.PlayClipAtPoint(EnemyDestroySounds[0], transform.position);

            // Sound and Item drops
            AudioSource.PlayClipAtPoint(EnemyDestroySounds[Random.Range(0, EnemyDestroySounds.Length)], transform.position);

            // Death of this AI
            CurrentHealth = 0;
            CurrentArmor = 0;
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
        _direction = new Vector2(-1, 0);                    // the direction set to left
        transform.localScale = new Vector3(1, 1, 1);        // resets sprite
        gameObject.SetActive(true);                         // shows this AI
        CurrentHealth = MaxHealth;                          // resets CurrentHealth
        CurrentArmor = MaxArmor;                            // resets CurrentArmor
        transform.localScale = new Vector2(0.75f, 0.75f);   // fixes resizing issue with touch screen overlay

        if (Enemy == EnemyType.Ogre)
            Swamp.SetActive(false);                         // hides BossAI Ogre's Swamp

        if (Enemy == EnemyType.Pirate)                      // resets BossAI Pirate's stats
        {
            MovementSpeed = 2;
            MaxProjectileCD = 2;
            ImmuneToDamage = true;
            ShotsFired = 0;
        }
    }
}
