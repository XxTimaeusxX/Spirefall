using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;
    public GameObject enemyPrefab;
    public TextMeshProUGUI waveText;
    
    // ADDED: Reference to the level generator
    public LevelGenerator levelGenerator;

    [Header("Infinite Wave Settings")]
    public float timeBetweenWaves = 5f;
    
    [Tooltip("How many enemies spawn on Wave 1")]
    public int baseEnemyCount = 3;
    [Tooltip("How many additional enemies are added each wave")]
    public int additionalEnemiesPerWave = 2;

    [Tooltip("How many enemies spawn per second on Wave 1")]
    public float baseSpawnRate = 1f; 
    [Tooltip("How much faster the spawn rate gets each wave")]
    public float spawnRateIncreasePerWave = 0.2f;

    [Header("Events")]
    public UnityEvent<int> OnWaveStarted;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        if (enemySpawner == null)
            enemySpawner = Object.FindFirstObjectByType<EnemySpawner>();

        // Look for the level generator if it wasn't assigned in the inspector
        if (levelGenerator == null)
            levelGenerator = Object.FindFirstObjectByType<LevelGenerator>();

        StartCoroutine(StartNextWave());
    }

    private void Update()
    {
        if (!isSpawning)
        {
            activeEnemies.RemoveAll(enemy => enemy == null);

            if (activeEnemies.Count == 0)
            {
                StartCoroutine(StartNextWave());
            }
        }
    }

    private IEnumerator StartNextWave()
    {
        isSpawning = true;
        currentWaveIndex++;

        OnWaveStarted?.Invoke(currentWaveIndex);
        Debug.Log($"Starting Wave {currentWaveIndex}!");
        
        if (waveText != null)
            waveText.text = $"Wave {currentWaveIndex}";

        // RANDOMIZE THE LEVEL HERE before the waiting period
        // (We don't do it on wave 1 so the player gets the default level shape first)
        if (levelGenerator != null && currentWaveIndex > 1)
        {
            levelGenerator.RandomizeLevel();
        }

        yield return new WaitForSeconds(timeBetweenWaves);

        int enemyCountThisWave = baseEnemyCount + (additionalEnemiesPerWave * (currentWaveIndex - 1));
        float spawnRateThisWave = baseSpawnRate + (spawnRateIncreasePerWave * (currentWaveIndex - 1));

        for (int i = 0; i < enemyCountThisWave; i++)
        {
            SpawnEnemy(enemyPrefab);
            yield return new WaitForSeconds(1f / spawnRateThisWave);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject prefab)
    {
        GameObject spawnedEnemy = enemySpawner.SpawnEnemy(prefab);

        if (spawnedEnemy != null)
        {
            activeEnemies.Add(spawnedEnemy);
        }
    }
}