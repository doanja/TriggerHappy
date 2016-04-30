using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
* Resource:
*
* The LevelManager class handles how points are awarded to the player. This class also
* updates the Camera Object's position according to the Player Object's position. 
* This class has a list of Checkpoint Objects. Upon reaching a Checkpoint, the debugSpawn
* Checkpoint will update, and when the Player is killed, they will respawn at the location of
* the debugSpawn. The amount of points the Player receives from reaching a Checkpoint is also
* calculated in this class. If the Player dies before reaching the next Checkpoint, this class
* will reset the amount of points back to the amount of points the Player has at the most
* recently acquired Checkpoint.
*/
public class LevelManager : MonoBehaviour {

    public static LevelManager Instance { get; private set; }   // instance of the LevelManager

    public Player Player { get; private set; }  // instance of the Player Object
    public CameraController Camera { get; private set; }    // instance of the Camera Object
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }  // timer

    // Calculates the amount of points the Player can receive upon reaching a checkpoint
    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(BonusCutOffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDifference) * BonusCutOffSeconds;
        }
    }

    private List<Checkpoint> _checkpoints;  // list of Checkpoint Objects
    private int _currentCheckpointIndex;    // the last acquired Checkpoint Object
    private DateTime _started;              // the start time upon acquiring a Checkpoint Object
    private int _savedPoints;               // the Player's total accumulated points

    public Checkpoint DebugSpawn;           // the Checkpoint Object that the Player will respawn at
    public int BonusCutOffSeconds;          // max time player has before reaching a Checkpoint Object
    public int BonusSecondMultiplier;       // calculates how many seconds * points

    // Use this for instantiation
    public void Awake()
    {
        _savedPoints = GameManager.Instance.Points;
        Instance = this;
    }

	// Use this for initialization
	public void Start () {

        // Checkpoint code
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        _currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1;

        Player = FindObjectOfType<Player>();
        Camera = FindObjectOfType<CameraController>();

        // Points code
        _started = DateTime.UtcNow;

        // Checkpoint and Score reseter for PointStars
        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();

        // Loops through each Checkpoint and assigns the pickups to a the previous Checkpoint
        foreach (var listener in listeners)
        {
            for (var i = _checkpoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkpoints[i].transform.position.x;
                if (distance < 0)
                    continue;

                _checkpoints[i].AssignObjectCheckpoint(listener);
                break;
            }
        }

        // Checkpoint code
#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#else
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);
#endif
    }
	
	// Update is called once per frame
	public void Update () {
        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
        if (isAtLastCheckpoint)
            return;

        var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;

        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
        _currentCheckpointIndex++;
        _checkpoints[_currentCheckpointIndex].PlayerHitCheckpoint();

        // When Checkpoint is acquired
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.Points; // calculates points incase player dies
        _started = DateTime.UtcNow;
	}

    /*
    * @param levelName, the next level transitioned to
    * Calls the StartCoroutine function to delay the text that appears after
    * reaching the end of the level.
    */
    public void GotoNextLevel(string levelName)
    {
        StartCoroutine(GotoNextLevelCo(levelName));
    }

    /*
    * @param levelName, the next level transitioned to
    * This method is called by GotoNextLevel to delay the floating text that appears.
    * This method calls the FinishLevel method from the Player class to disable the
    * Player's controllers, colliders, and [TODO: animations, sound]. This method
    * then transitions the Player to the next level, levelName, or back to the
    * StartScreen by default.
    */
    private IEnumerator GotoNextLevelCo(string levelName)
    {
        Player.FinishLevel();   // invoke the FinishLevel method
        GameManager.Instance.AddPoints(CurrentTimeBonus); // adds all the Player's points together

        // Displays text
        FloatingText.Show("Level Complete!", "CheckpointText", new CenteredTextPositioner(.2f));
        yield return new WaitForSeconds(1);

        //FloatingText.Show(string.Format("{0} points!", GameManager.Instance.Points), "CheckpoinText", new CenteredTextPositioner(.1f));
        //yield return new WaitForSeconds(5f);

        if (string.IsNullOrEmpty(levelName))
            Application.LoadLevel("StartScreen");   // loads start screen by default if no levelname is given
        else
            Application.LoadLevel(levelName);       // loads specified level
    }

    // Invokes the Startcourtine method to call the KillPlayer method in the Player class
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }

    private IEnumerator KillPlayerCo()
    {
        Player.Kill();
        Camera.IsFollowing = false;
        yield return new WaitForSeconds(2f);

        Camera.IsFollowing = true;

        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(Player);

        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);
    }
}
