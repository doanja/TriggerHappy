using UnityEngine;

/*
 * This class is a collection of all Projectiles classes that handles the
 * trajectory the projectile, what happens when the projectile is destroyed,
 * and any secondary effects that the projectile has.
 */
public class AllProjectiles : Projectile, ITakeDamage {

    /* All Projectiles Must Have */
    public GameObject DestroyedEffect;  // the effect played upon the destruction of this GameObject
    public int PointsToGiveToPlayer;    // the amount of points the Player Object receives
    public float TimeToLive;            // the amount of time this GameObject lives
    public AudioClip DestroySound;      // the sound played when this GameObject dies
    private float _speed = 5.0f;               // the velocity of the projectile

    /* PathedProjectile */
    public Transform _destination;     // the end point of the projectile

    /* SinProjectile */
    public float Frequency = 6.0f;      // speed of sine movement
    public float Magnitude = 1.0f;      // size of sine movement
    private Vector3 Axis;
    private Vector3 Pos;

    /* HomingProjectile */
    private Player Player;              // instance of the player class
    public float TurnSpeed = 1.5f;      // speed of which the projectile can adjust its position
    private Vector3 Target;             // the target's location

    /* Lock On Projectiles */
    private bool IsTargetInRange;       // used to determine if the target is in range of this projectile
    public float DetectionRadius;       // the distance between the target and this projectile

    /* Projectile Secondary Effects */
    public bool CanFreeze;              // slows enemy movement speed
    public bool CanConfuse;             // reverse enemy direction
    public bool CanParalyze;            // disable enemy from firing projectiles
    public bool CanDisable;             // prevents enemy from moving and firing projectiles

    private EnemyAI Enemy;              // instance of the EnemyAI

    public enum ProjectileType          // projectile behavior based on type
    {
        SimpleProjectile,               // standard projectile that travels straight
        PathedProjectile,               // projectile that follows a set path towards its destination
        SinProjectile,                  // projectile that oscillates
        HomingProjectile,               // projectile that pursues its target (can be invaded)
        LockOnProjectile                // projectile that can lock onto a target and cannot be invaded [WIP]
    }
    public ProjectileType Proj;         // instance of an ProjectileType, used to determine Projectile behavior

    // Constructor
    public void Initialize(Transform destination, float speed)
    {
        _destination = destination;
        _speed = speed;
    }

    // Use this for initialization
    void Start () {
        if (Proj == ProjectileType.SinProjectile)
        {
            Pos = transform.position;
            Axis = transform.up;
        }

        Enemy = FindObjectOfType<EnemyAI>();    // find instance of the EnemyAI
        Player = FindObjectOfType<Player>();    // finds instances of the player
    }
	
	// Update is called once per frame
	void Update () {

        // Handles how long the projectory stays active
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

        // SimpleProjectiles
        if (Proj == ProjectileType.SimpleProjectile || Proj == ProjectileType.LockOnProjectile)
        {
            // Handles trajectory
            transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);

            if (Proj == ProjectileType.LockOnProjectile)
            {
                // Determines if target is in range
                IsTargetInRange = Physics2D.OverlapCircle(transform.position, DetectionRadius, 1 << LayerMask.NameToLayer("Player"));

                // Checks to see if target is in range of the projectile
                if (IsTargetInRange)
                {
                    // Handles movement of this projectile
                    transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, Speed * Time.deltaTime);
                    return;
                }
            }
        }
        
        // PathedProjectile
        if (Proj == ProjectileType.PathedProjectile)
        {
            /*
            // Handles trajectory
            transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);
            var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared > 0.1f * 0.01f)
                return;
            
            DestroyProjectile();
            */

            // Handles movement of this GameObject
            transform.position = Vector3.MoveTowards(transform.position, _destination.transform.position, _speed * Time.deltaTime);
            return;
        }

        // SinProjectiles
        if (Proj == ProjectileType.SinProjectile)
        {
            // Handles trajectory
            Pos += transform.right * Time.deltaTime * _speed;
            transform.position = Pos + Axis * Mathf.Sin(Time.time * Frequency) * Magnitude;
        }

        // Homing Projectiles
        if (Proj == ProjectileType.HomingProjectile)
        {
            // Calculates Player position and rotations to limit homing accuracy
            Target = Player.transform.position;
            Vector2 Direction = Target - transform.position;
            Quaternion Rotation = Quaternion.LookRotation(Direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, TurnSpeed * Time.deltaTime);

            // Projectile Movement
            transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
        }
    }


    // Function that indicates that displays range of the DetectionRadius
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, DetectionRadius);

        if (_destination == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _destination.position);
    }

    /*
    * @param damage, the amount of damage the projectile deals
    * @param instigator, the GameObject inflicting damage
    * Function to handle damage and awarding the player points
    */
    public void TakeDamage(int damage, GameObject instigator)
    {
        if (PointsToGiveToPlayer != 0)
        {
            var projectile = instigator.GetComponent<Projectile>();
            if (projectile != null && projectile.Owner.GetComponent<CharacterController>() != null)
            {
                GameManager.Instance.AddPoints(PointsToGiveToPlayer);
                FloatingText.Show(string.Format("+{0}!", PointsToGiveToPlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
            }
        }
    }

    /*
    * @param other, the other GameObject
    * Destroys this projectile when it collides with other
    */
    protected override void OnCollideOther(Collider2D other)
    {
        DestroyProjectile();
    }

    /*
    * @param other, the other GameObject
    * @param takeDamage, the amount of damage that the other GameObject receives
    * On collision, the other GameObject takes damage
    */
    protected override void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {
        // Handles what happens when the enemy is frozen
        if (other.CompareTag("Enemies") && CanFreeze == true)
        {
            Enemy.MovementSpeed /= 2;
            Enemy.SpriteColor.color = Color.cyan;
        }

        // Handles what happens when the enemy is confused
        if(other.CompareTag("Enemies") && CanConfuse == true)
        {
            Enemy.Reverse();
        }

        // Handles what happens when the enemy is paralyzed
        if (other.CompareTag("Enemies") && CanParalyze == true)
        {
            Enemy.CanFireProjectiles = false;
        }

        // Handles what happens when the enemy is disabled
        if (other.CompareTag("Enemies") && CanDisable == true)
        {
            Enemy.MovementSpeed = 0;
            Enemy.CanFireProjectiles = false;
        }

        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile(); // destroys the projectile
    }

    // Function that handles what happens when the projectile is destroyed
    private void DestroyProjectile()
    {
        // Handles effects
        if (DestroyedEffect != null)
            Instantiate(DestroyedEffect, transform.position, transform.rotation);

        // Handles Sound
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);

        // Destroys this GameObject
        Destroy(gameObject);
    }
}
