using UnityEngine;

/*
* Resource:
*
* Allows GameObjects to deal damage to the Player Object by invoking the Player Class' 
* TakeDamage method and overrides the CharacterController2D to knock back the Player Object.
*/
public class GiveDamageToPlayer : MonoBehaviour {

    public int DamageToGive = 10;   // damage dealt to the player's health

    // Calculates how far to knock the player back, depending on the velocity of the object
    private Vector2
        _lastPosition,
        _velocity;

    public void LateUpdate()
    {
        _velocity = (_lastPosition - (Vector2) transform.position) / Time.deltaTime;
        _lastPosition = transform.position;
    }

    // Handles how what this GameObject does on collision with the Player Object.
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>(); // instance of the Player object
        if (player == null) // does nothing if another GameObject collides withs this GameObject
            return;

        // Invokes the Player class' TakeDamage passing in this GameObject's DamageToGive
        player.TakeDamage(DamageToGive, gameObject); 
        var controller = player.GetComponent<CharacterController2D>(); // instance of the Charactercontroller2D class
        var totalVelocity = controller.Velocity + _velocity; // calculates new velocity

        // Overrides character controller to knockback the player
        controller.SetForce(new Vector2(
           -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 3, 5, 20),
           -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 3, 2, 15)));
    }
}
