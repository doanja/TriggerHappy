using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Class to determine where the checkpoint is located on the level 
 * and where the player respawns after death.
 */
public class Checkpoint : MonoBehaviour {

    private List<IPlayerRespawnListener> _listeners;

	// Use this for initialization
	public void Awake() {
        _listeners = new List<IPlayerRespawnListener>();
	}

    // Method calls the PlayerHitCheckPointCo method when a checkpoint is acquired
    public void PlayerHitCheckpoint()
    {
        /* https://youtu.be/9YBcuSWik9w?list=PLt_Y3Hw1v3QSFdh-evJbfkxCK_bjUD37n&t=1744 */
        StartCoroutine(PlayerHitCheckpointCo(LevelManager.Instance.CurrentTimeBonus));
    }

    // Method to invoke CheckpointText from GameHUD
    private IEnumerator PlayerHitCheckpointCo(int bonus)
    {
        FloatingText.Show("Checkpoint!", "CheckpointText", new CenteredTextPositioner(.5f));
        yield return new WaitForSeconds(.5f);
        FloatingText.Show(string.Format("+{0} time bonus!", bonus), "CheckpointText", new CenteredTextPositioner(.5f));
    }

    public void PlayerLeftCheckpoint()
    {

    }

    // Respawns the player at the last acquired checkpoint
    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);

        foreach (var listener in _listeners)
            listener.OnPlayerRespawnInThisCheckpoint(this, player);
    }

    public void AssignObjectCheckpoint(IPlayerRespawnListener listener)
    {
        _listeners.Add(listener);
    }
}
