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

    public GameObject gate;

    public bool HalfDamage;

    // Projectiles
    public float FireRate = 4;                  // cooldown time after firing a projectile
    public Projectile Projectile;               // this GameObject's projectile
    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    public AudioClip ShootSound;                // the sound when this GameObject shoots a projectile

    // Overlap Circle
    private Player Player;                  // instance of the player class
    public float PlayerDetectionRadius;     // the distance between the Player Object and this GameObject
    private bool IsPlayerInRange;           // used to determine if the Player Object is in range of this GameObject
    public LayerMask DetectThisLayer;       // determines what this GameObject is colliding with

    // Helper
    public GameObject SpawnEffect;
    public GameObject HelperPrefab;
    public float ActionDelay1;
    public float ActionDelay2;
    public float Cooldown1;                  // used to count down the time before an action can be taken by the AI
    public float Cooldown2;

    // Slime
    private Vector3 _currentPosition;   // current position of the AI
    public Transform[] SpawnPoints;

    public enum EnemyType
    {
        Slime,
        Tubby,
        Queen
    }
    public EnemyType Enemy;

    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        CurrentHealth = MaxHealth;                              // sets current health to maximum health
    }
	
	// Update is called once per frame
	void Update () {

        // Handles basic movement
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            Reverse();

        // Variable used to determine if the DetectThisLayer overlaps with the Circle
        IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);


        if (Enemy == EnemyType.Slime)
        {
            // Checks cooldown count
            if ((Cooldown1 -= Time.deltaTime) > 0 || (Cooldown2 -= Time.deltaTime) > 0)
                return;

            StartCoroutine(CountdownRegen());  // starts countdown before regenerating health
            Cooldown1 = ActionDelay1;            // resets the cooldown

            StartCoroutine(CountdownSummon());
            Cooldown2 = ActionDelay2;
        }

        /*
        // Checks to see when the AI can jump
        if (_controller.CanJump && IsPlayerInRange == true)
            _controller.Jump();

        
        if (Enemy != EnemyType.Queen)
        {
            // AI will only summon crystal prefab under these conditions
            if (CurrentHealth >= (MaxHealth * .50))
            {
                // Checks cooldown count
                if ((Cooldown -= Time.deltaTime) > 0)
                    return;

                StartCoroutine(CountDownSummonCrystal());   // summons the orb
                Cooldown = SummonTime;                      // resets the cooldown
            }

            // AI moves, and changes color to indicate that it is more OP
            if (CurrentHealth <= MaxHealth * .50)
            {
                SpriteColor.color = Color.yellow;
                HalfDamage = true;
                MovementSpeed = 7;
            }

            // AI begins shooting, and moves faster
            if (CurrentHealth <= MaxHealth * .25)
            {
                SpriteColor.color = Color.blue;
                HalfDamage = true;
                MovementSpeed = 4;

                // Checks cooldown count
                if ((Cooldown -= Time.deltaTime) > 0)
                    return;

                // Casts rays to detect player
                var raycast = Physics2D.Raycast(transform.position, _direction, 10, 1 << LayerMask.NameToLayer("Player"));
                if (!raycast)
                    return;

                var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
                projectile.Initialize(gameObject, _direction, _controller.Velocity);

                if (ShootSound != null)
                    AudioSource.PlayClipAtPoint(ShootSound, transform.position);

                Cooldown = FireRate;
            }
        }
        

        if (Enemy == EnemyType.Tubby)
        {
            // Variable used to determine if the DetectThisLayer overlaps with the Circle
            IsPlayerInRange = Physics2D.OverlapCircle(transform.position, PlayerDetectionRadius, DetectThisLayer);

            // Handles the event that the Player is in range of dectiong by the AI
            if (IsPlayerInRange)
            {
                // Checks to see when the AI can jump
                if (_controller.CanJump)
                {
                    _controller.Jump();
                    JumpCounter++;
                }
            }

            // AI begins shooting, and moves faster
            if (CurrentHealth <= MaxHealth * .25)
            {
                SpriteColor.color = Color.red;
                HalfDamage = true;
                MovementSpeed = 4;
            }

            if(JumpCounter == 3)
            {
                StartCoroutine(CountDownSummonHelpers());   // summons the orb
                Cooldown = SummonTime;                      // resets the cooldown
            }

        }
            */
    }

    // Function to summon Crystal the AI's current position
    public void SummonHelper()
    {
        Instantiate(HelperPrefab, transform.position, transform.rotation);
        Instantiate(SpawnEffect, transform.position, transform.rotation);
    }

    // Function to count down time before this AI can summon another Helper
    IEnumerator CountdownSummon()
    {
        yield return new WaitForSeconds(ActionDelay2);
        SummonHelper();
        yield return 0;
    }

    IEnumerator CountdownRegen()
    {
        yield return new WaitForSeconds(ActionDelay1);
        CurrentHealth = MaxHealth;
        yield return 0;
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

            if (Enemy == EnemyType.Slime)
            {
                for (int i = 0; i < SpawnPoints.Length; i++)
                    Instantiate(HelperPrefab, SpawnPoints[i].position, SpawnPoints[i].rotation);
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
