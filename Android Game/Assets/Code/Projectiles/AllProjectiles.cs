using UnityEngine;

public class AllProjectiles : Projectile, ITakeDamage {

    /* All Projectiles Must Have */
    public int Damage;                  // the damage this projectile inflicts
    public GameObject DestroyedEffect;  // the effect played upon the destruction of this GameObject
    public int PointsToGiveToPlayer;    // the amount of points the Player Object receives
    public float TimeToLive;            // the amount of time this GameObject lives
    public AudioClip DestroySound;      // the sound played when this GameObject dies

    /* PathedProjectile */
    private Transform _destination;     // the end point of the projectile
    private float _speed;               // the velocity of the projectile

    /* SinProjectile */
    public float ProjectileTravelSpeed = 5.0f;
    public float Frequency = 6.0f;      // Speed of sine movement
    public float Magnitude = 1.0f;      // Size of sine movement
    private Vector3 Axis;
    private Vector3 Pos;

    /* Status Effect Bonuses */
    // freeze, reverse direction
    public bool CanFreeze;
    public bool CanConfuse;
    private EnemyAI Enemy;

    public enum ProjectileType          // projectile behavior based on type
    {
        SimpleProjectile,
        PathedProjectile,
        SinProjectile
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

        Enemy = FindObjectOfType<EnemyAI>();
        //CanFreeze = false;
        //CanConfuse = false;
    }
	
	// Update is called once per frame
	void Update () {

        // The amount of time this projectile lives
        if ((TimeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }

        // SimpleProjectiles
        if (Proj == ProjectileType.SimpleProjectile)
        {
            // Handles the speed of the projectile
            transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + Speed) * Time.deltaTime), Space.World);
        }
        
        /* PathedProjectile */
        if (Proj == ProjectileType.PathedProjectile)
        {
            // Handles the travel path of the object
            transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);
            var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared > 0.1f * 0.01f)
                return;

            DestroyProjectile();
        }

        // SinProjectiles
        if (Proj == ProjectileType.SinProjectile)
        {
            // Handles the speed of the projectile
            Pos += transform.right * Time.deltaTime * ProjectileTravelSpeed;

            // Handles the Position of the projectile
            transform.position = Pos + Axis * Mathf.Sin(Time.time * Frequency) * Magnitude;
        }
    }

    /*
    * @param damage, the amount of damage
    * @param instigator, the GameObject inflicting damage
    * Method allows this projectile to deal damage to another GameObject
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
    * Instance of this projectile is destroyed
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

        takeDamage.TakeDamage(Damage, gameObject);
        DestroyProjectile(); // destroys the projectile
    }

    // Method to destroy the projectile
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
