using UnityEngine;

public class PainterElizabeth : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{

    // Parameters
    public float MovementSpeed;         // travel speed of this GameObject
    public GameObject DestroyedEffect;  // the destroyed effect
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject
    public GameObject[] Portals;        // portals Elizabeth uses to summon her minions

    // Character Essentials    
    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    // Health
    public int MaxHealth = 150;             // maximum health of the this GameObject
    public int Health { get; private set; } // this GameObject's current health    
    public GameObject HealthBar;            // health bar to display this GameObject's Health

    // Sound
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed

    // Jump
    private int jumpRNG;
    public int maxRNG;

    // End Level Portal
    public GameObject gate;

    // Spawner   
    public GameObject enemy;                // The enemy prefab to be spawned.
    public int maxSpawnRNG;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        Health = MaxHealth;

        gate.SetActive(false);                                  // makes the end level portal invisible       
    }

    // Update is called once per frame
    public void Update()
    {
        // Calculates the jumpRNG
        int spawnRNG = Random.Range(0, maxSpawnRNG);

        // Handles Jumping
        if (spawnRNG == 1)
        {
            Spawn();
        }


        // Calculates the jumpRNG
        int jumpRNG = Random.Range(0, maxRNG);

        // Handles Jumping
        if(jumpRNG == 2 && _controller.CanJump == true)
        {
            _controller.Jump();
        }

        // Sets the x-velocity of this GameObject
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
        {
            _direction = -_direction; // switches direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
            gate.SetActive(true);                       // makes end level portal visible
            HealthBar.SetActive(false);                 // hides the health bar
            Portals[0].SetActive(false);
            Portals[1].SetActive(false);
            Portals[2].SetActive(false);

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
        _direction = new Vector2(-1, 0);                // the direction set to left
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.SetActive(true);                     // shows this GameObject                                      

        // Resets health
        Health = MaxHealth;                             // sets current health to the GameObject's max health
    }

    public void Spawn()
    {        
        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
