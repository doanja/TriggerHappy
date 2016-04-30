using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
    //Jeremy's shit
	public float maxSpeed = 10f;
    public float jumpForce = 450f;
    float groundRadius = 0.2f;
    public bool grounded = false;
    bool facingRight = true;
    public Transform groundCheck;
	public LayerMask whatIsGround;
    Animator anim;

    //John's shit
    public Projectile Projectile;
    public GameObject StandingOn { get; private set; }
    public Vector3 PlatformVelocity { get; private set; }
    private Vector3 _activeGlobalPlatformPoint, _activeLocalPlatformPoint;

    // www.youtube.com/watch?v=hkLAdo9ODDs
    // Handles moving platforms [DOES NOT WORK]
    private void HandlePlatforms()
    {
        if (StandingOn != null)
        {
            var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
            var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
            if (moveDistance != Vector3.zero)
                transform.Translate(moveDistance, Space.World);
            PlatformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
        }
        else
            PlatformVelocity = Vector3.zero;
        StandingOn = null;
    }


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        // Used for animation
		anim.SetBool("Ground", grounded);
		anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
		float move = Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(move));

        // Handles moving platforms [DOES NOT WORK]
        if(StandingOn != null)
        {
            _activeGlobalPlatformPoint = transform.position;
            _activeGlobalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
        }

        // Physically handles character movement
        GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

		if (move>0 && !facingRight){ //flip character directionally
			Flip();
		}

		else if(move<0 && facingRight){
			Flip();
		}
	}

    // Update is called once per frame
    void Update(){
		if(grounded && Input.GetKeyDown(KeyCode.Space)){
			anim.SetBool("Ground", false);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
		}
	}

    // Flips the character's sprite to indicate which direction the player is facing
    void Flip(){
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
