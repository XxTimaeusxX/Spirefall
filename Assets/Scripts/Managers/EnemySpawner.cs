using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The Enemy Prefab to spawn")]
    public GameObject enemyPrefab;
    
    [Tooltip("How many seconds between each spawn")]
    public float spawnInterval = 2f;

    [Header("Level Reference")]
    public LevelGenerator levelData;

    private float timer = 0f;

    void Start()
    {
        // Automatically find the LevelGenerator if not assigned
        if (levelData == null) 
        {
            levelData = Object.FindFirstObjectByType<LevelGenerator>();
        }
    }

    void Update()
    {
        if (levelData == null || enemyPrefab == null) return;

        // Run the timer
        timer += Time.deltaTime;
        
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            
            // Reset the timer. You could also randomize this interval slightly for less predictable spawns!
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // 1. Pick a random lane based on the number of sides the shape currently has
        int randomLane = Random.Range(0, levelData.polygonSides);

        // 2. Set the initial spawn position (X and Y are 0 for a moment, the Enemy script will fix them)
        Vector3 spawnPos = new Vector3(0, 0, levelData.tubeLength);

        // 3. Spawn the enemy
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // 4. Tell the enemy which lane to travel on and give it the level data
        EnemyMovement enemyScript = newEnemy.GetComponent<EnemyMovement>();
        if (enemyScript != null)
        {
            enemyScript.currentLane = randomLane;
            enemyScript.levelData = this.levelData; // Pass the reference so it doesn't have to search for it
        }
    }
}
