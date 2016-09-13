using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    public Sprite WeaponSprite;

    // Weapon Parameters
    public Projectile Projectile;                   // the Player Object's projectile
    public float FireRate;                          // cooldown after firing a projectile
    public Transform ProjectileFireLocation;        // the location of which the projectile is fired at
    public GameObject FireProjectileEffect;         // the effect played when the Player Object is shooting
    public float Cooldown;                          // Player object is able to fire when this equals the FireRate
    public AudioClip PlayerShootSound;

    // Use this for initialization
    void Start()
    {
        WeaponSprite = GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = WeaponSprite;

    }
}