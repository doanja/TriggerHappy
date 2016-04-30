using UnityEngine;

/*
* Resource:
*
* 
*/
public class Projectile : MonoBehaviour {
    
    public float Speed;
    public LayerMask CollisionMask;

    public GameObject Owner { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 InitialVelocity { get; private set; }

    public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity)
    {
        // match the direction of the projectile with the direciton they're facing
        transform.right = direction;

        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
        OnInitialized();
    }
    
    protected virtual void OnInitialized()
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if((CollisionMask.value & (1 << other.gameObject.layer)) == 0)
        {
            OnNotCollideWith(other);
            return;
        }

        var isOwner = other.gameObject == Owner;
        if (isOwner)
        {
            OnCollideOwner();
            return;
        }
       
        var takeDamage = (ITakeDamage)other.GetComponent(typeof(ITakeDamage));
        if(takeDamage != null)
        {
            OnCollideTakeDamage(other, takeDamage);
            return;
        }
        OnCollideOther(other);
    }
	protected virtual void OnNotCollideWith(Collider2D other)
    {

    }

    protected virtual void OnCollideOwner()
    {

    }

    protected virtual void OnCollideTakeDamage(Collider2D other, ITakeDamage takeDamage)
    {

    }

    protected virtual void OnCollideOther(Collider2D other)
    {

    }
}
