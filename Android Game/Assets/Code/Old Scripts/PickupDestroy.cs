using UnityEngine;
using System.Collections;

public class PickupDestroy : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
