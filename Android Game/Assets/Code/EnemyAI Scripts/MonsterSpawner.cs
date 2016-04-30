using UnityEngine;

/*
* Resource: Adapted from ElizabethAI & ProjectileSpawner
* 
* This GameObject behaves like the ProjectileSpawner and fires Projectiles.
* The main difference from this and the ProjectileSpawner is that it is
* destroyable. This means that it implements the ITakeDamage and IPlayerRespawnListener
* interfaces, so when this GameObject is destroyed it will respawn at its _startPosotion.
*/
public class MonsterSpawner : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{   
    // Parameters
    public float Speed;                     // the travel speed of the projectile towards its destination
    public float FireRate;                  // the rate of shots the projectile will be fired at
    private float Cooldown;                 // the cooldown before firing another shot
    public Projectile Projectile;           // the projectile shot
    public GameObject ProjectileSpawnEffect;// effect played when spawning the projectile
    public GameObject DestroyedEffect;      // the destroyed effect of this GameObject
    public Transform ProjectileFireLocation;// the location of which the projectile is fired at
    public int PointsToGivePlayer;          // points awarded to the player upon killing this GameObject

    // Sound
    public AudioClip SpawnProjectileSound;  // the sound of the projectile spawning
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed   

    // Character Essentials
    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    // Health
    public int MaxHealth = 100;                 // maximum health of the this GameObject
    public int Health { get; private set; }     // this GameObject's current health    

    //public Animator anim;                   // animation

    // Use this for initialization
    void Start()
    {       
        _controller = GetComponent<CharacterController2D>();
        _direction = new Vector2(-1, 0);    // this GameObject will move the left upon initialization
        _startPosition = transform.position;
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Spawner code
        if ((Cooldown -= Time.deltaTime) > 0)
            return;

        // Instantiates the projectile, and initilializes the speed, and direction of the projectile
        var projectile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        projectile.Initialize(gameObject, _direction, _controller.Velocity);
        Cooldown = FireRate; // time frame, when projectiles can be shot from this GameObject

        // Handles projectile effects
        if (ProjectileSpawnEffect != null)
            Instantiate(ProjectileSpawnEffect, transform.position, transform.rotation);

        // Sound
        if (SpawnProjectileSound != null)
            AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);
        /*
        if (anim != null)
            anim.SetTrigger("Fire");*/
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
        // Re-initializes this GameObject
        _direction = new Vector2(-1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.SetActive(true);                     // shows this GameObject

        // Resets health
        Health = MaxHealth;                             // sets current health to the GameObject's max health
    }
}
