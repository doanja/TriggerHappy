using System.Collections;
using UnityEngine;

/*
* Adapted from GhostAI
*
* This class will cause GameObjects to create a overlap circle, which is used to detect
* any GameObjects set as the CollisionMask and will cause this GameObject to pursue the
* CollisionMask until they are out of range, or unless this GameObject's health reaches
* zero.  
*/
public class FrogAI : MonoBehaviour
{

    public float jumpTime = 2f;
    public float chance;
    public Vector2 jumpforce;
    public Player target;

    void Start()
    {
        StartCoroutine("Move");
        if (target == null)
            target = FindObjectOfType<Player>();
    }

    void Update()
    {

    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpTime);

            int i = 1;

            if(Random.value<chance)
            {
                if (target.transform.position.x > transform.position.x)
                    i = 1;
                else
                    i = -1;
            }
            else
            {
                if (target.transform.position.x > transform.position.x)
                    i = -1;
                else
                    i = 1;

                GetComponent<Rigidbody2D>().velocity = new Vector2(jumpforce.x * i, jumpforce.y);
            }
        }
    }
}
