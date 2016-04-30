using UnityEngine;
using System.Collections;
//www.youtube.com/watch?v=DKQCplBJ4yE&index=16&list=PLQzQtnB2ciXRvU5GRn4mTLlz21kSVg9XN
public class SimpleEnemyScript : MonoBehaviour {

    public float maxSpeed = 10f;
    public float velocity = 1f;
    public Transform sightStart;
    public Transform sightEnd;
    public LayerMask detectWhat;
    public bool colliding;
    //Animator anim;

	// Use this for initialization
	void Start () {
        //anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        // Use this for animation
        //float move = Input.GetAxis("Horizontal");
        //anim.SetFloat("Speed", Mathf.Abs(move));

        // Handles the speed of the enemy
        GetComponent<Rigidbody2D>().velocity = new Vector2(velocity*maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

        // Handles collision of the enemy
        colliding = Physics2D.Linecast(sightStart.position, sightEnd.position, detectWhat);
        if (colliding)
        {
            // Mirrors the enemy and change the direction path of the enemy
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            velocity *= -1;
        }
	}
}
