using UnityEngine;

public class InstaKill : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;

        LevelManager.Instance.KillPlayer();
    }
}
