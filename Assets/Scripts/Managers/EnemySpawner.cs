using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level Reference")]
    public LevelGenerator levelData;

    void Start()
    {
        if (levelData == null) 
        {
            levelData = Object.FindFirstObjectByType<LevelGenerator>();
        }
    }

    // Changed from void to GameObject
    public GameObject SpawnEnemy(GameObject enemyPrefab)
    {
        if (levelData == null || enemyPrefab == null) return null;

        int randomLane = Random.Range(0, levelData.polygonSides);
        Vector3 spawnPos = new Vector3(0, 0, levelData.tubeLength);
        
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyMovement enemyScript = newEnemy.GetComponent<EnemyMovement>();
        if (enemyScript != null)
        {
            enemyScript.currentLane = randomLane;
            enemyScript.levelData = this.levelData; 
        }

        // Return the newly created enemy
        return newEnemy;
    }
}
