using UnityEngine;

/*
*
*
*/
public class RaycastShootUp : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{

    // Parameters   
    public float MovementSpeed;         // travel speed of this GameObject
    public float FireRate = 1;          // cooldown time after firing a projectile
    private float Cooldown;             // the amount of time this GameObject can shoot projectiles
    public Projectile Projectile;       // this GameObject's projectile
    public GameObject DestroyedEffect;  // the destroyed effect of this GameObject   
    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    // Sound
    public AudioClip ShootSound;            // the sound when this GameObject shoots a projectile
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed    

    // Character Essentials
    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject   

    // Direction of travel of the Projectile
    private Vector2 _fireVector;
    public int x;
    public int y;

    // Player Checks
    private Player player;          // instance of the player class
    public LayerMask CollisionMask; // determines what this GameObject is colliding with
    public float PlayerRange;       // the distance between the Player Object and this GameObject
    public bool playerInRange;      // used to determine if the Player Object is in range of this GameObject

    // Health
    public int MaxHealth = 100;                 // maximum health of the this GameObject
    public int Health { get; private set; }     // this GameObject's current health    

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        _controller = GetComponent<CharacterController2D>();
        _direction = new Vector2(-1, 0);    // this GameObject will move the left upon initialization
        _fireVector = new Vector2(x, y);
        _startPosition = transform.position;
        Health = MaxHealth;
    }

    // Update is called once per frame
    public void Update()
    {
        // Sets the x-velocity of this GameObject
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Variable used to determines if the CollisionMask overlaps with the Circle
        playerInRange = Physics2D.OverlapCircle(transform.position, PlayerRange, CollisionMask);

        // Spawner code
        if ((Cooldown -= Time.deltaTime) > 0)
            return;

        Cooldown = FireRate;

        // If the Player Object is in range of this GameObject, and they are facing away, move this GameObject towards the PlayerObject
        if (playerInRange)
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            projectile.Initialize(gameObject, _fireVector, _controller.Velocity);
            Cooldown = FireRate; // time frame, when projectiles can be shot from this GameObject

            // Handles Sound
            if (ShootSound != null)
                AudioSource.PlayClipAtPoint(ShootSound, transform.position);
        }

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
        {

            _direction = -_direction; // switches direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }        
    }

    // Visual indicator for line of travel for the projectile & PlayerRange
    public void OnDrawGizmos()
    {
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
        // Re-initializes this GameObject's direction, and start position
        _direction = new Vector2(-1, 0);
        transform.localScale = new Vector3(1, 1, 1);        
        gameObject.SetActive(true);                     // shows this GameObject       

        // Resets health
        Health = MaxHealth;                             // sets current health to the GameObject's max health
    }
}
