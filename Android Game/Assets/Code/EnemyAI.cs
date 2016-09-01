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
    public AudioClip EnemyDetectedSound;        // sound played when EnemyType has detected the Player

    private int refCounterRNG;          // RNG reference variable for EnemyDestroySounds
    private int refItemRNG;             
    public enum EnemyType               // enemy behavior based on type
    {
        Patrol,
        PatrolShoot
    }
    public EnemyType Enemy;             // instance of an EnemyType, used to determine AI behavior
    
    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        Health = MaxHealth;
    }
	
	// Update is called once per frame
	void Update () {

        // Handles selection of random AudioClip selected from EnemyDestroySounds array
        int counterRNG = Random.Range(0, EnemyDestroySounds.Length);
        refCounterRNG = counterRNG; // updates the refCounterRNG variable

        // Handles selection of random AudioClip selected from ItemDroplist array
        int itemRNG = Random.Range(0, ItemDroplist.Length);
        refItemRNG = itemRNG;       // updates the refItemRNG variable

        if (Enemy == EnemyType.Patrol || Enemy == EnemyType.PatrolShoot)
        {
            /* Handles basic movement */

            // Sets the x-velocity of this GameObject
            _controller.SetHorizontalForce(_direction.x * MovementSpeed);

            // Checks to see if this GameObject is colliding with something in the same direction
            if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            {
                _direction = -_direction; // switches direction
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            /* Projectiles */
            if(Enemy == EnemyType.PatrolShoot)
            {
                // Handles when this GameObject cannot shoot
                if ((Cooldown -= Time.deltaTime) > 0)
                    return;

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
            AudioSource.PlayClipAtPoint(EnemyDestroySounds[refCounterRNG], transform.position);
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
