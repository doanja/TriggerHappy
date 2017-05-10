using UnityEngine;

public class Lightbulb : MonoBehaviour {

    private BossAI Boss;
    private Player Player;
    public int DamageGivenToBoss = 100;
    public AudioClip PickupSound;

    // Use this for initialization
    void Start () {
        Boss = FindObjectOfType<BossAI>();
        Player = FindObjectOfType<Player>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        Boss.TakeDamage(DamageGivenToBoss, this.gameObject);

        // Handles Sound
        if (PickupSound != null)
            AudioSource.PlayClipAtPoint(PickupSound, transform.position);

        Destroy(gameObject);
    }

    }
