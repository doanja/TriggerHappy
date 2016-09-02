using UnityEngine;

/* 
* Resource: https://www.youtube.com/watch?v=re6fookKraU&index=28&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n 
* 
* This class handles how a generic enemy AI preforms. This class implements an instance of the
* CharacterController2D, which is used to check collisions, and the speed/velocity of this GameObject.
* This GameObject will also be able to shoot projectiles at the player. The player is rewarded points 
* for killing this GameObject, and can be awarded poitns form killing this GameObject's projectiles.
*/
public class ElizabethBossAI : MonoBehaviour, ITakeDamage
{

    public float speed;                 // travel speed of this GameObject
    public float FireRate = 1;          // cooldown time after firing a projectile
    public Projectile Projectile;       // this GameObject's projectile
    public GameObject DestroyedEffect;  // the destroyed effect of this GameObject
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    // Sound
    public AudioClip ShootSound;            // the sound when this GameObject shoots a projectile
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed    

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject
    private float projectileCooldown;                   // the amount of time this GameObject can shoot projectiles

    public Transform ProjectileFireLocation;    // the location of which the projectile is fired at
    //public GameObject FireProjectileEffect;         // the effect played when the player is shooting

    // Health
    public int MaxHealth = 100;                 // maximum health of the this GameObject
    public int Health { get; private set; }     // this GameObject's current health    


    private Player player;              // instance of the player class
    public float detectionRange;        // the distance between the Player Object and this GameObject
    public bool playerInRange;        // used to determine if the Player Object is in range of this GameObject
    public LayerMask CollisionMask;     // determines what this GameObject is colliding with


    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _direction = new Vector2(-1, 0);    // this GameObject will move the left upon initialization
        _startPosition = transform.position;
        Health = MaxHealth;
    }

    // Update is called once per frame
    public void Update()
    {

        // Sets the x-velocity of this GameObject
        _controller.SetHorizontalForce(_direction.x * speed);

        // Variable used to determines if the CollisionMask overlaps with the Circle
        playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, CollisionMask);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
        {
            _direction = -_direction; // switches direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // Handles when this GameObject cannot shoot
        if ((projectileCooldown -= Time.deltaTime) > 0)
            return;

        // If the player is in range of the turret's attack range, shoot 
        if (playerInRange && !player.IsDead)
        {
            // Instantiates the projectile, and initilializes the speed, and direction of the projectile
            var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
            projectileCooldown = FireRate; // time frame, when projectiles can be shot from this GameObject

            // Handles Sound
            if (ShootSound != null)
                AudioSource.PlayClipAtPoint(ShootSound, transform.position);
        }
    }

    // Method draws a sphere indicating the range of view of this GameObject
    public void OnDrawGizmosSelected()
    {
        // Detection range for this GameObject and the Player Object
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, detectionRange);
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
}
