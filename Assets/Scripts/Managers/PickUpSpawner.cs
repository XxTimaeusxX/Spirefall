using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject pickupObject;
    public WaveManager waveManager;
    private GameObject currentPickupInstance;
    
    [Header("Level Reference")]
    public LevelGenerator levelData;

    [Tooltip("If true, the pickup respawns every wave. If false, maybe every N waves.")]
    public bool respawnEveryWave = true;
    public int respawnFrequency = 2; // Only used if respawnEveryWave is false

    void Start()
    {
        // Find our managers if they are not assigned
        if (waveManager == null) waveManager = FindFirstObjectByType<WaveManager>();
        if (levelData == null) levelData = FindFirstObjectByType<LevelGenerator>();

        if (waveManager != null)
        {
            waveManager.OnWaveStarted.AddListener(OnNewLevelStarted);
        }
        
        // Wait a tiny frame for LevelGenerator to setup its arrays on Start, then spawn
        Invoke(nameof(RespawnPickup), 0.1f);
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnWaveStarted.RemoveListener(OnNewLevelStarted);
        }
    }

    private void OnNewLevelStarted(int waveIndex)
    {
        if (pickupObject == null || levelData == null) return;

        // Automatically respawn the pickup depending on the wave rule
        if (respawnEveryWave || waveIndex % respawnFrequency == 0)
        {
            RespawnPickup();
        }
    }

    private void RespawnPickup()
    {
        if (pickupObject == null || levelData == null) return;
        if (levelData.EdgeOffsets == null || levelData.EdgeOffsets.Length == 0) return;

        // 1. Pick a random lane just like EnemySpawner does
        int randomLane = Random.Range(0, levelData.polygonSides);

        // 2. Calculate the PERFECT starting position based on the LevelGenerator's math
        Vector3 spawnPos = new Vector3(
            levelData.centerPoint.x + levelData.EdgeOffsets[randomLane].x,
            levelData.centerPoint.y + levelData.EdgeOffsets[randomLane].y,
            levelData.tubeLength // Spawn at the far end of the tube
        );
        
        // Match the rotation flat to the lane
        Quaternion spawnRot = levelData.EdgeRotations[randomLane];

        // 3. Spawn or reactivate the pickup
        if (currentPickupInstance == null)
        {
            currentPickupInstance = Instantiate(pickupObject, spawnPos, spawnRot);
        }
        else
        {
            // Reset position, rotation, and turn it back on
            currentPickupInstance.transform.position = spawnPos;
            currentPickupInstance.transform.rotation = spawnRot;
            currentPickupInstance.SetActive(true);
        }
    }
}
