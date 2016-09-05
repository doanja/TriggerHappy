using UnityEngine;

public class BossAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    /* Beginning of All Enemy Type Parameters */
    public float MovementSpeed;         // travel speed of this GameObject
    public GameObject DestroyedEffect;  // the destroyed effect
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    public int MaxHealth = 100;                         // maximum health of the this GameObject
    public int CurrentHealth { get; private set; }      // this GameObject's current health    

    public AudioClip[] EnemyDestroySounds;      // sound played when this GameObject is destroyed
    public GameObject[] ItemDroplist;           // array of items that the enemy can drop
    /* End of All Enemy Type Parameters */
    
    // Use this for initialization
    void Start () {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        CurrentHealth = MaxHealth;                              // sets current health to maximum health
    }
	
	// Update is called once per frame
	void Update () {
	
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
            Instantiate(ItemDroplist[Random.Range(0, ItemDroplist.Length)], transform.position, Quaternion.identity);

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
