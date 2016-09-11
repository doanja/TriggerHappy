using UnityEngine;

/*
* Resources: 
*
* This is the player class which handles the Player Object's physics, and controls of the player.
* The player has a set amount of health which can be decremented by another GameObject if they have
* the scripts necessary to deal damage to the Player Object. If the Player object's health reaches 
* zero, the Kill method will be invoked, and the player will respawn at the last acquired checkpoint. 
* This class has an instance of the Charactercontroller2D which overrides Unity's generic Physics.
* This allows the Player Object's parameters to be overriden by other classes that can affect how 
* the Player Object's physics react under certain conditions, such as terrain, and special effects
* caused by other GameObjects.
*/
public class Player : MonoBehaviour, ITakeDamage
{

    private bool _isFacingRight;                    // checks if the Player Object's sprite is facing right
    public CharacterController2D _controller;      // instance of the CharacterController2D
    private float _normalizedHorizontalSpeed;       // x-direction speed: -1 = left, 1 = right
    private float _normalizedVerticalSpeed;         // y-direction speed: -1 = down, 1 = up

    public float MaxSpeed = 8;                      // max speed of the Player Object
    public float SpeedAccelerationOnGround = 10f;   // how quickly the Player Object goes from moving to not moving on ground
    public float SpeedAccelerationInAir = 5f;       // how quickly the Player Object goes from moving to not moving on air
    public int MaxHealth = 100;                     // maximum health of the Player Object
    public GameObject OuchEffect;                   // effect played when the Player Object is receiving damage
    
    // Sound
    public AudioClip PlayerHealthSound, PlayerDeathSound;
    public AudioClip[] JumpSounds;
    public AudioClip[] PlayerHitSounds;


    // Health & Lives
    public int Health { get; private set; }         // Player Object's current health
    public bool IsDead { get; private set; }        // determines if the user can control the Player Object
    private LifeManager lifeSystem;                 // instance of the LifeManager

    // Ladder
    public bool onLadder;                           // determines if the Player Object is overlapping with a ladder
    private float GravityStore;                     // variable used to store the Player Object's default gravity

    // Animation
    public Animator Animator;

    // Touch Control Movement
    public int hInput = 0;
    public int vInput = 0;

    public PlayerWeapon Weapon;

    // RNG Reference Variables
    private int refCounterRNG;
    private int refHitRNG;

    // Use this for initialization
    public void Awake()
    {
        _controller = GetComponent<CharacterController2D>();    // initializes an instance of the CharacterController2D
        _isFacingRight = transform.localScale.x > 0;            // ensure Player Object's sprite is facing to the right
        Health = MaxHealth;                                     // initializes Player Object's health to max health
        GravityStore = _controller.DefaultParameters.Gravity;   // stores the Player's starting gravity

        //lifeSystem = FindObjectOfType<LifeManager>();
    }

    // Update is called once per frame
    public void Update()
    {
        Weapon.Cooldown -= Time.deltaTime; // When this reaches 0, they player can shoot again
        
        if (!IsDead)
            HandleInput(); // Handles what the player press (left, right, jump, shoot)
            
        // Changes movement factor depending on if the Player object is falling in midair, or when it is grounded
        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;

        // Handles horizontal velocity + interpolates/scales the horizontal movement of the Player      
        _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        if (onLadder)
        {
            _controller.SetVerticalForce(Mathf.Lerp(_controller.Velocity.y, _normalizedVerticalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        }

        // Animation
        Animator.SetBool("IsGrounded", _controller.State.IsGrounded);
        Animator.SetBool("IsDead", IsDead);
        //Animator.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x) / MaxSpeed);
        Animator.SetFloat("Speed", Mathf.Abs(hInput));

        // Touch Controls
        //MoveHorizontal(hInput);
        //MoveVertical(vInput);

        // Handles selection of random AudioClip selected from JumpSounds array
        int counterRNG = Random.Range(0, (JumpSounds.Length));
        refCounterRNG = counterRNG; // updates the refCounterRNG variable

        // Handles selection of random AudioClip selected from PlayerHitSounds array
        int hitRNG = Random.Range(0, (PlayerHitSounds.Length));
        refHitRNG = hitRNG; // updates the refHitRNG variable
    }

    /*
    * Resource: https://www.youtube.com/watch?v=lHb213yRP-Y&index=33&list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n
    *
    * This method will disable user input, and ignore the features in game
    * such as physics, collision, death, etc.
    */
    public void FinishLevel()
    {
        enabled = false;
        _controller.enabled = false;
    }

    /*
    * Method invoked when the player's health reaches zero.
    */
    public void Kill()
    {
        // Sound
        AudioSource.PlayClipAtPoint(PlayerDeathSound, transform.position);

        _controller.HandleCollisions = false;       // player will fall through object
        GetComponent<Collider2D>().enabled = false; // collider2D.enabled = false;
        IsDead = true;
        Health = 0;                                 // sets Player Object's health to 0                              

        _controller.SetForce(new Vector2(0, 20));   // and bounces player up
    }

    /*
    * @param spawnPoint, the location where the player is respawned
    * Method called to respawn the player, and re-initializes the Player's beginning state.
    */
    public void RespawnAt(Transform spawnPoint)
    {
        // Handles which direction the player is facing upon Respawn
        if (!_isFacingRight)
            Flip();

        IsDead = false;                             // player is not dead
        GetComponent<Collider2D>().enabled = true;  // collider2D.enabled = true;
        _controller.HandleCollisions = true;        // sets collisions to true again
        Health = MaxHealth;                         // sets current health to the Player object's max health
        onLadder = false;

        transform.position = spawnPoint.position;   // respawns the player at the spawnPoint

        //lifeSystem.TakeLife();                      // decrements lives on the LifeManager
    }

    /*
    * @param damage, the damage taken by the Player object
    * @param instigator, the GameObject initializing the damage dealt to the Player object
    * Method to decrement the player's health when they are hit/damaged by an enemy/trap
    */
    public void TakeDamage(int damage, GameObject instigator)
    {
        // Floating text
        FloatingText.Show(string.Format("-{0}", damage), "PlayerTakeDamageText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60f));

        // Sound
        AudioSource.PlayClipAtPoint(PlayerHitSounds[refHitRNG], transform.position);

        // Decrement's the player's health
        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        // If the player's Health reaches zero, call KillPlayer
        if (Health <= 0)
            LevelManager.Instance.KillPlayer();
    }

    /*
    * @param health, the health of the player
    * @param instigator, the GameObject initializing the health recovery
    * Method that allows the Player object to recover lost health.
    */
    public void GiveHealth(int health, GameObject instigator)
    {
        // Sound
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);

        // Floating text
        FloatingText.Show(string.Format("+{0}", health), "PlayerGotHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60f));

        // Increment's the player's health
        Health = Mathf.Min(Health + health, MaxHealth);
    }

    /*
    * Method that allows the user to control the Player object.
    * Left = A
    * Right = D
    * Space = Jump
    * Left Click = Shoot
    *    
    * TODO: Up = Climb Ladder
    */
    private void HandleInput()
    {
        // Handles right direction, and changing the Player object's sprite to match
        if (Input.GetKey(KeyCode.D))
        {
            _controller.DefaultParameters.Gravity = GravityStore;   // reset gravity     
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }

        // Handles left direction, and changing the Player object's sprite to match
        else if (Input.GetKey(KeyCode.A))
        {
            _controller.DefaultParameters.Gravity = GravityStore;   // reset gravity     
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
                Flip();
        }

        else if (onLadder)
        {
            // Moves the player upwards on the ladder
            if (Input.GetKey(KeyCode.W))
            {
                _normalizedVerticalSpeed = 1;   // Y-direction speed = positive = up
                _normalizedHorizontalSpeed = 0;
                _controller.DefaultParameters.Gravity = 0;
            }

            // Moves the player downwards on the ladder
            else if (Input.GetKey(KeyCode.S))
            {
                _normalizedVerticalSpeed = -1;  // Y-direction speed = negative = down
                _normalizedHorizontalSpeed = 0;
                _controller.DefaultParameters.Gravity = 0;
            }

            // If the player is hanging on the ladder & no input has been detected
            else
            {
                _normalizedHorizontalSpeed = 0;
                _normalizedVerticalSpeed = 0;  // Y-direction speed = 0 = on ladder/not moving    
                _controller.DefaultParameters.Gravity = 0;
            }
        }

        // If the player is not pressing anything
        else
        {
            _normalizedHorizontalSpeed = 0;
            _normalizedVerticalSpeed = 0;
            _controller.DefaultParameters.Gravity = GravityStore;   // reset gravity                
        }

        // Handles jumping
        if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
            
            // Sound
            AudioSource.PlayClipAtPoint(JumpSounds[refCounterRNG], transform.position);
        }
        
        // Handles shooting
        if (Input.GetMouseButton(0))
            FireProjectile();     
        
    }

    /*
    * Method that determines when the Player object can fire. 
    * Handles instantiation and initialize, direction of the projectile and resets canFireIn.
    */
    public void FireProjectile()
    {
        // If the cooldown is still counting down to 0, the player cannot fire.
        if (Weapon.Cooldown > 0)
            return;

        if (Weapon.FireProjectileEffect != null)
        {
            // Plays the effect in the direction the player is facing
            var effect = (GameObject)Instantiate(Weapon.FireProjectileEffect, Weapon.ProjectileFireLocation.position, Weapon.ProjectileFireLocation.rotation);
            effect.transform.parent = transform;
        }

        // Check direction to ensure projectiles are firing in the same direction as the Player class
        var direction = _isFacingRight ? Vector2.right : -Vector2.right;

        // Instantiates the projectile, and initilializes the speed, and direction of the projectile
        var projectile = (Projectile)Instantiate(Weapon.Projectile, Weapon.ProjectileFireLocation.position, Weapon.ProjectileFireLocation.rotation);
        projectile.Initialize(gameObject, direction, _controller.Velocity);
        Weapon.Cooldown = Weapon.FireRate; // time frame, when projectiles can be shot from this GameObject      

        // Sound
        AudioSource.PlayClipAtPoint(Weapon.PlayerShootSound, transform.position);

        // Animation
        Animator.SetTrigger("Shoot");
    }

    // Method to vertically flip the Player object's sprite    
    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
    }

    // Function invoked by TouchControls.cs to allow horizontal movement of the player
    public void MoveHorizontal(int direction)
    {
        _controller.SetHorizontalForce(direction * 7.5f);       
        if (direction == 1)
        {
            if (!_isFacingRight)
                Flip();
        }
        else if (direction == -1)
        {
            if (_isFacingRight)
                Flip();
        }
    }

    // Function invoked by TouchControls.cs to allow vertical movement of the player
    public void MoveVertical(int direction)
    {
        if (onLadder)
        {
            _controller.SetVerticalForce(direction * 6.0f);
            _normalizedHorizontalSpeed = 0;
            _controller.DefaultParameters.Gravity = 0;
        }
    }

    // Function invoked by TouchControls.cs to make the player jump
    public void TouchJump()
    {
        if (_controller.CanJump)
        {
            _controller.Jump();
            // Sound
            AudioSource.PlayClipAtPoint(JumpSounds[refCounterRNG], transform.position);
        }
    }

    // Function invoked by TouchControls.cs to fire a projectile
    public void TouchShoot()
    {
        FireProjectile();
    }

    public void MoveRight()
    {
        //_controller.DefaultParameters.Gravity = GravityStore;   // reset gravity     
        _normalizedHorizontalSpeed = 1;
        if (!_isFacingRight)
            Flip();
        Update();
    }

    public void MoveLeft()
    {
        //_controller.DefaultParameters.Gravity = GravityStore;   // reset gravity     
        _normalizedHorizontalSpeed = -1;
        if (_isFacingRight)
            Flip();
        Update();
    }
}