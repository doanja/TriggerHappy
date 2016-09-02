using UnityEngine;

public class TimeToLive : MonoBehaviour {

    public float lifetime;            // the amount of time this GameObject lives    
    public GameObject BlowupEffect;     // the blowup effect
    public AudioClip BlowupSound;     // sound played when this GameObject collides with the Player

    // Update is called once per frame
    void Update () {
        // The amount of time this projectile lives
        if ((lifetime -= Time.deltaTime) <= 0)
        {
            AudioSource.PlayClipAtPoint(BlowupSound, transform.position);
            Instantiate(BlowupEffect, transform.position, transform.rotation);
            Destroy(this.gameObject);
            return;
        }
    }
}
