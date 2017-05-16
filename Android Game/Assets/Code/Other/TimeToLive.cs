using UnityEngine;

public class TimeToLive : MonoBehaviour {

    public float lifetime;              // the amount of time this GameObject lives    
    public GameObject BlowupEffect;     // the blowup effect
    public AudioClip BlowupSound;       // sound played when this GameObject's lifetime < 0
    public bool SummonOnDeath;          // if true, instantiates a GameObject upon death
    public GameObject SpawnedEnemy;     // GameObject to be instantiated

    // Update is called once per frame
    void Update () {

        // The amount of time this GameObject lives
        if ((lifetime -= Time.deltaTime) <= 0)
        {
            AudioSource.PlayClipAtPoint(BlowupSound, transform.position);
            Instantiate(BlowupEffect, transform.position, transform.rotation);

            if (SummonOnDeath == true)
                Instantiate(SpawnedEnemy, transform.position, transform.rotation);

            Destroy(this.gameObject);
            return;
        }
    }
}
