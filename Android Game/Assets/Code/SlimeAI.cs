using UnityEngine;

public class SlimeAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    // Parameters
    public float MovementSpeed;         // travel speed of this GameObject
    public GameObject DestroyedEffect;  // the destroyed effect
    public int PointsToGivePlayer;      // points awarded to the player upon killing this GameObject

    // Character Essentials    
    public CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _direction;                 // the x-direction of this GameObject
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    // Health
    public int MaxHealth = 100;             // maximum health of the this GameObject
    public int Health { get; private set; } // this GameObject's current health    

    // Sound
    public AudioClip EnemyDestroySound;     // sound played when this GameObject is destroyed

    // Boss
    public GameObject bossPrefab;
    public int cloneCounter = 3;
    public float minSize;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController2D>();    // instance of Charactercontroller2D
        _direction = new Vector2(-1, 0);                        // this GameObject will move the left upon initialization
        _startPosition = transform.position;                    // starting position of this GameObject
        Health = MaxHealth;
    }

    // Update is called once per frame
    void Update () {

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
            if(cloneCounter > 0) {
                SlimeAI clone1 = Instantiate(bossPrefab, new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z), transform.rotation) as SlimeAI;
                SlimeAI clone2 = Instantiate(bossPrefab, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), transform.rotation) as SlimeAI;

                clone1.transform.localScale = new Vector3(transform.localScale.y * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z);
                clone1.GetComponent<SlimeAI>().MaxHealth = MaxHealth / 2;
                clone1.cloneCounter--;
                clone2.transform.localScale = new Vector3(transform.localScale.y * 0.5f, transform.localScale.y * 0.5f, transform.localScale.z);
                clone2.GetComponent<SlimeAI>().MaxHealth = MaxHealth / 2;
                clone2.cloneCounter--;
            }
                       
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
}
