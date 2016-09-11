using UnityEngine;

/*
* Resource: 
*
* This class handles how much health the Player class recover upon colliding
* with a GameObject with the GiveHealth script applied.
*/
public class GiveHealth : MonoBehaviour, IPlayerRespawnListener
{

    public GameObject Effect;       // special effects upon colliding with the GameObject
    public int HealthToGive;        // amount of health the player receives from this GameObject
    public AudioClip PickupSound;   // sound played when the player collides this GameObject

    // Handles what happens to the GameObject
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Creates an instance of the Player Class
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        // Handles Sound
        if (PickupSound != null)
            AudioSource.PlayClipAtPoint(PickupSound, transform.position);

        player.GiveHealth(HealthToGive, gameObject); // increases the player's health
        
        // Handles effects
        Instantiate(Effect, transform.position, transform.rotation);

        // Floating text appears when picked up
        FloatingText.Show(string.Format("+{0}!", HealthToGive), "PlayerGotHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        gameObject.SetActive(false);
    }

    // Method used to respawn this GameObject after the player respawns at the given checkpoint
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}
