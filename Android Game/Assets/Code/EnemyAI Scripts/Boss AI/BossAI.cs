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
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    public int MaxHealth = 100;                         // maximum health of the this GameObject
    public int CurrentHealth { get; private set; }      // this GameObject's current health    
    public GameObject HealthBar;                        // health bar to display this AI's current health

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public SpriteRenderer SpriteColor;          // reference to the AI's sprite color
    private Vector3 _currentPosition;           // current position of the AI
    public GameObject gate;

    public bool HalfDamage;                     // damage taken by this enemy is halved when true

    // Projectiles
    public float MaxProjectileCD = 4;           // time needed to fire again
    public float CurrentProjectileCD;           // current time before being able to fire projectiles
    public Projectile Projectile;               // this GameObject's projectile
    public Transform[] ProjectileFireLocation;  // the location of which the projectile is fired at
    public AudioClip ShootSound;                // the sound when this GameObject shoots a projectile

    // Overlap Circle
    private Player Player;                  // instance of the player class
    public float PlayerDetectionRadius;     // the distance between the Player Object and this GameObject
    private bool IsPlayerInRange;           // used to determine if the Player Object is in range of this GameObject
    public LayerMask DetectThisLayer;       // determines what this GameObject is colliding with

    // RNG Variables
    private int CurrentRNGCount;            // random number variable

    // Helper
    public GameObject[] Helpers;            // array of helpers that can be summoned
    public GameObject SpawnEffect;          // effect shown when the Helper is Spawned
    public float MaxActionCD1;              // max countdown before an action can be preformed
    public float MaxActionCD2;              // max countdown before an action can be preformed
    public float CurrentActionCD1;          // used to countdown the time before an action can be taken by the AI
    public float CurrentActionCD2;          // used to countdown the time before an action can be taken by the AI

    // Slime
    public Transform[] SpawnPoints;         // locations where Helpers can be spawned
    public GameObject HealEffect;

    // Ogre
    public bool RageActive;                 // keeps track when Ogre has its rings active
    public GameObject Barrier;

    public enum EnemyType
    {
        Slime,
        Ogre
    }
    public EnemyType Enemy;

    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        CurrentHealth = MaxHealth;                              // sets current health to maximum health

        if (Enemy == EnemyType.Ogre)
            Barrier.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        // Handles selection of random number within MaxRNGCount
        int random = Random.Range(0, (Helpers.Length));
        CurrentRNGCount = random; // updates the CurrentRNGCountRNG

        // Handles basic movement
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            Reverse();

        // Variable used to determine if the DetectThisLayer overlaps with the Circle
        IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);

        // Elizabeth Slime
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

        // Elizabeth Ogre
        if (Enemy == EnemyType.Ogre)
        {
            if(RageActive == false)
            {
                Debug.Log("Waiting to summon barrier");
                if ((CurrentActionCD2 -= Time.deltaTime) > 0)                // ogre barrier is not active, call the summon
                    return;

                StartCoroutine(CountdownBarrierDown());     // starts countdown before summoning the green barrier
                CurrentActionCD2 = MaxActionCD2;            // resets the cooldown
            }
            
            if (RageActive == true)
            {
                Debug.Log("Waiting for barrier to die");
                // Checks cooldown count
                if ((CurrentActionCD1 -= Time.deltaTime) > 0)
                    return;

                Debug.Log("Rage active, waiting to set barrier to false");
                StartCoroutine(CountdownBarrierUp());   // starts countdown before regenerating health
                CurrentActionCD1 = MaxActionCD1;        // resets the cooldown
            }
        }
    }
    // Time before Barrier Goes Down
    IEnumerator CountdownBarrierUp()
    {
        yield return new WaitForSeconds(MaxActionCD1);  // kills barrier after x time
        RageActive = false;                             // set check to false
        Barrier.SetActive(false);                    // set the barrier to false
        Debug.Log("Barrier down");
        yield return 0;
    }

    // Sets Barrier to Active
    IEnumerator CountdownBarrierDown()
    {
        yield return new WaitForSeconds(MaxActionCD2);
        RageActive = true;
        Barrier.SetActive(true);
        Debug.Log("Barrier up");
        yield return 0;
    }

    // Function to countdown time before BossAI heals to full health
    IEnumerator CountdownRegen()
    {
        yield return new WaitForSeconds(MaxActionCD1);
        CurrentHealth = MaxHealth;
        Instantiate(HealEffect, transform.position, transform.rotation);
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
        Debug.Log("Summoning Helper");
        //Instantiate(Helpers[CurrentRNGCount], transform.position, transform.rotation);
        Instantiate(Helpers[CurrentRNGCount], SpawnPoints[CurrentRNGCount].position, SpawnPoints[CurrentRNGCount].rotation);
        Instantiate(SpawnEffect, transform.position, transform.rotation);
    }

    // Function called by AI to instantiate a projectile and fire it in its direction
    public void FireProjectile()
    {
        for(int i = 0; i < ProjectileFireLocation.Length; i++)  // handles multiple projectile firing locations
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation[i].position, ProjectileFireLocation[i].rotation);
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
        }
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
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null)
        {
            if(HalfDamage == true)
                projectile.Damage /= 2;
        }
    }

    // Function that indicates that displays range of the PlayerDetectionRadius
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, PlayerDetectionRadius);
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
            // SLIME ONLY: Summon Slime Helpers upon death
            if (Enemy == EnemyType.Slime)
            {
                for (int i = 0; i < SpawnPoints.Length; i++)
                    Instantiate(Helpers[CurrentRNGCount], SpawnPoints[i].position, SpawnPoints[i].rotation);
            }

            gate.SetActive(true);                       // makes end level portal visible
            HealthBar.SetActive(false);                 // hides the health bar

            // Sound and Item drops
            AudioSource.PlayClipAtPoint(EnemyDestroySounds[Random.Range(0, EnemyDestroySounds.Length)], transform.position);

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
