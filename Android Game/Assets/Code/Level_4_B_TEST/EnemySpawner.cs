using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject EnemyPrefab;
    public float MaxSpawnRate = 5f;

	// Use this for initialization
	void Start () {
        Invoke("SpawnEnemy", MaxSpawnRate);
        InvokeRepeating("IncreaseSpawnRate", 0f, 30f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnEnemy()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        GameObject EnemyPrefabClone = (GameObject)Instantiate(EnemyPrefab);
        EnemyPrefab.transform.position = new Vector2(Random.Range(min.x, max.x), max.y);

        ScheduleNextEnemySpawn();
    }

    void ScheduleNextEnemySpawn()
    {
        float spawnInSeconds;

        if (MaxSpawnRate > 1f)
        {
            spawnInSeconds = Random.Range(1f, MaxSpawnRate);
        }
        else
            spawnInSeconds = 1f;

        Invoke("SpawnEnemy", spawnInSeconds);
    }

    void IncreaseSpawnRate()
    {
        if (MaxSpawnRate > 1f)
            MaxSpawnRate--;
        if (MaxSpawnRate == 1f)
            CancelInvoke("IncreaseSpawnRate");
    }

    public void ScheduleEnemySpawner()
    {
        // reset max spawn rate
        MaxSpawnRate = 5f;

        Invoke("SpawnEnemy", MaxSpawnRate);

        // increase spawn rate every 30s
        InvokeRepeating("IncreaseSpawnRate", 0f, 30f);
    }

    public void UnscheduleEnemySpawner()
    {
        CancelInvoke("SpawnEnemy");
        CancelInvoke("IncreaseSpawnRate");
    }
}
