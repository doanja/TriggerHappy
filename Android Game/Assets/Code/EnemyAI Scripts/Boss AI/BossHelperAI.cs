using UnityEngine;

public class BossHelperAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
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
    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed

    private Player Player;                      // instance of the player class
    public GameObject SpawnEffect;

    /* EnemySpawner */
    public EnemyAI SpawnedEnemy;     // the enemy prefab to be spawned
    public float SpawnTime = 3f;        // how long between each spawn
    public Transform[] SpawnPoints;     // an array of the spawn points this enemy can spawn from

    public enum AIType
    {
        EnemySpawner
    }
    public AIType AI;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        CurrentHealth = MaxHealth;                              // sets current health to maximum health

        Player = FindObjectOfType<Player>();                    // finds instances of the player

        if (AI == AIType.EnemySpawner)
        {
            // calls the Spawn function after a delay of the SpawnTime and then continue to call after the same amount of time.
            InvokeRepeating("Spawn", SpawnTime, SpawnTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Handles basic movement
        _controller.SetHorizontalForce(_direction.x * MovementSpeed);

        // Checks to see if this GameObject is colliding with something in the same direction
        if ((_direction.x < 0 && _controller.State.IsCollidingLeft) || (_direction.x > 0 && _controller.State.IsCollidingRight))
            Reverse();
    }

    // Function called by EnemySpawner AIs that spawns a enemy at set locations
    public void Spawn()
    {
        // soes not spawn enemies when the player or the AI has zero health
        if (Player.Health <= 0 || CurrentHealth == 0)
            return;

        // find a random index between zero and one less than the number of spawn points
        int spawnPointIndex = Random.Range(0, SpawnPoints.Length);

        // create an instance of the enemy prefab at the randomly selected spawn point's position and rotation
        Instantiate(SpawnedEnemy, SpawnPoints[spawnPointIndex].position, SpawnPoints[spawnPointIndex].rotation);
    }

    // Function to change direction and velocity
    public void Reverse()
    {
        // switches direction and flips the sprite
        _direction = -_direction;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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
