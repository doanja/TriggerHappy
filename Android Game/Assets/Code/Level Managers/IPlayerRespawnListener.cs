/*
* Interface used to determine which GameObjects to respawn (setActive to true),
* depending on the Checkpoint Object that the Player Object has reached.
*/

public interface IPlayerRespawnListener {

    void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player);
}
