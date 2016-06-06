using UnityEngine;

public class PickupWeapon : MonoBehaviour, IPlayerRespawnListener
{
    private Player player;          // reference to the Player
    public GameObject Effect;       // special effects upon colliding with the GameObject
    public AudioClip PickupSound;   // sound played when the player collides this GameObject                                

    // New Weapon Parameters
    public Sprite NewWeaponSprite;
    public Projectile NewProjectile;                   // the Player Object's projectile
    public float NewFireRate;                          // cooldown after firing a projectile
    public Transform NewProjectileFireLocation;        // the location of which the projectile is fired at
    public GameObject NewFireProjectileEffect;         // the effect played when the Player Object is shooting
    public float NewCooldown;                          // Player object is able to fire when this equals the FireRate
    public AudioClip NewPlayerShootSound;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();      
    }

    /*
   * @param other, the other GameObject
   * Handles what happens to the GameObject
   */
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Does nothing if another GameObject collides with this GameObject
        if (other.GetComponent<Player>() == null)
            return;

        // Handles Sound
        if (PickupSound != null)
            AudioSource.PlayClipAtPoint(PickupSound, transform.position);

        // Handles effects
        Instantiate(Effect, transform.position, transform.rotation);

        // Set new weapon parameters
        player.Weapon.WeaponSprite = NewWeaponSprite;
        player.Weapon.Projectile = NewProjectile;
        player.Weapon.FireRate = NewFireRate;
        //player.Weapon.ProjectileFireLocation = NewProjectileFireLocation;
        player.Weapon.FireProjectileEffect = NewFireProjectileEffect;
        player.Weapon.Cooldown = NewCooldown;
        player.Weapon.PlayerShootSound = NewPlayerShootSound;

        gameObject.SetActive(false); // hides this GameObject        
    }

    /*
    * @param checkpoint, the most recent checkpoint the Player Object has acquired
    * @param player, the Player Object
    * Method used to respawn this GameObject after the player respawns at the given checkpoint
    */
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true); // shows this GameObject
    }
}
