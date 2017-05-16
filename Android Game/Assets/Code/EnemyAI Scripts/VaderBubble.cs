using System.Collections;
using UnityEngine;

public class VaderBubble : MonoBehaviour
{

    private Player Player;
    public float MovementSpeed;
    public float lifetime;              // the amount of time this GameObject lives after collision with the player
    private float lifetime2 = 10f;      // the amount of time this GameObject can live after it is summoned
    public AudioClip BlowupSound;       // sound played when this GameObject's lifetime < 0
    private bool inBubble;

    // Use this for initialization
    void Start()
    {
        Player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, MovementSpeed * Time.deltaTime);

        // The amount of time this projectile lives
        if ((lifetime2 -= Time.deltaTime) <= 0)
        {
            AudioSource.PlayClipAtPoint(BlowupSound, transform.position);
            Destroy(this.gameObject);
            return;
        }

        if (inBubble == true)
        {
            transform.position = new Vector2(Player.transform.position.x, Player.transform.position.y);
            Player.transform.position = new Vector2(Player.transform.position.x, 4);

            // The amount of time this projectile lives
            if ((lifetime -= Time.deltaTime) <= 0)
            {
                AudioSource.PlayClipAtPoint(BlowupSound, transform.position);
                Destroy(this.gameObject);
                return;
            }
        }
    }
    
    // Handles collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        Player.Status = Player.PlayerStatus.Paraylyzed;
        Player.StartCoroutine(Player.CountdownDebuff());
        Player.CanFireProjectiles = false;
        Player.MaxSpeed = 0;

        Player.SpriteColor.color = Color.black;

        inBubble = true;

        StartCoroutine(CountdownDestroy());
    }

    // Function to countdown time before BossAI heals to full health
    IEnumerator CountdownDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        yield return 0;
    }
}
