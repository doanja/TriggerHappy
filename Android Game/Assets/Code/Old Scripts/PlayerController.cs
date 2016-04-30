using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public Vector2 jumpVector;
	public bool isGrounded;
	public Transform grounder;
	public float radiuss;
	public LayerMask ground;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKey(KeyCode.A)){ // move left
			//GetComponent<Rigidbody2D>().velocity = new Vector2 (-speedForce,GetComponent<Rigidbody2D>().velocity.y);
			transform.localScale = new Vector3((float)-0.7,(float)0.7,(float)0.7);
			Vector3 position = this.transform.position;
			position.x -= (float)0.1;
			this.transform.position = position;
		}
		else if(Input.GetKey(KeyCode.D)){ // move right
			//GetComponent<Rigidbody2D>().velocity = new Vector2 (speedForce,GetComponent<Rigidbody2D>().velocity.y);
			transform.localScale = new Vector3((float)0.7,(float)0.7,(float)0.7);
			Vector3 position = this.transform.position;
			position.x += (float)0.1;
			this.transform.position = position;
		}
		else{
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0,GetComponent<Rigidbody2D>().velocity.y);
		}
		
		isGrounded = Physics2D.OverlapCircle(grounder.position, radiuss, ground);
		
		if(Input.GetKey(KeyCode.W) && isGrounded == true){ // jump
			//GetComponent<Rigidbody2D>().AddForce(jumpVector,ForceMode2D.Force);
			
			Vector3 position = this.transform.position;
			position.y += (float)2.0;
			this.transform.position = position;
		}
		
	}
}
