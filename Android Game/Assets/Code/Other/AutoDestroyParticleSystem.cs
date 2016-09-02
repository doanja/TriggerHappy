using UnityEngine;

/*
* Attach to a particle system, animates the particle, after animation, 
* it will destroy the GameObject that this is applied to.
*/
public class AutoDestroyParticleSystem : MonoBehaviour {

    private ParticleSystem _particleSystem;

	// Use this for initialization
	public void Start () {
        _particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	public void Update () {
        if (_particleSystem.isPlaying)
            return;

        Destroy(gameObject);
	}
}
