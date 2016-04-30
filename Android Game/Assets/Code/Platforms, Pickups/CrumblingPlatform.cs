using UnityEngine;
using System.Collections;

/*
* Script to make platforms fall and disappear.
*/
public class CrumblingPlatform : MonoBehaviour, IPlayerRespawnListener
{
    private Rigidbody2D _rigidbody2D;
    public float FallDelay;                     // rate at which this GameObject's Y-direction changes
    public float DisappearDelay;                // rate at which this GameObject's visibility gets set to false

    private CharacterController2D _controller;  // has an instance of the CharacterController2D
    private Vector2 _startPosition;             // the initial spawn position of this GameObject

    // Use this for initialization
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    /*
    * @param other, the other object colliding with this GameObject
    * Handles Collision
    */
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;
       
        StartCoroutine(Fall());
        StartCoroutine(Delay());
    }

    // Applies Gravity
    IEnumerator Fall()
    {
        yield return new WaitForSeconds(FallDelay);
        _rigidbody2D.isKinematic = false;        
        yield return 0;
    }

    // Delays the GameObject's visibility
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(DisappearDelay);
        gameObject.SetActive(false);
        yield return 0;
    }

    /*
    * @param checkpoint, the last checkpoint the Player Object has acquired
    * @param player, the Player Object
    * Method used to respawn this GameObject after the player respawns at the given checkpoint
    */
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {        
        gameObject.SetActive(true);         // shows this GameObject       
        _rigidbody2D.isKinematic = true;    // turns of Unity Physics
    }

}
