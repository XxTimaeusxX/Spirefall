using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum EnemyState { MovingForward, OnEdge }

    [Header("Enemy Settings")]
    public float moveSpeed = 10f;
    public int currentLane = 0;
    
    [Header("Edge Walking Settings")]
    public float edgeMoveSpeed = 15f; 
    public float edgeHopDelay = 0.5f; // Wait time before hopping to next lane

    [Header("Level Reference")]
    public LevelGenerator levelData;

    private EnemyState currentState = EnemyState.MovingForward;
    private Shipmovement playerScript; // Reference to track player position
    private float nextHopTime = 0f;

    void Start()
    {
        if (levelData == null) levelData = Object.FindFirstObjectByType<LevelGenerator>();
        playerScript = Object.FindFirstObjectByType<Shipmovement>(); // Find player to chase later

        if (levelData != null && levelData.EdgeOffsets != null)
        {
            SnapToCurrentLane();
        }
        else
        {
            Debug.LogError("LevelGenerator not found or not initialized!");
        }
    }

    void Update()
    {
        if (currentState == EnemyState.MovingForward)
        {
            // Move steadily along the Z-axis
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
        }
        else if (currentState == EnemyState.OnEdge)
        {
            HandleEdgeMovement();
        }
    }

    private void HandleEdgeMovement()
    {
        // Hop to a new lane periodically
        if (Time.time >= nextHopTime)
        {
            DecideNextLane();
            nextHopTime = Time.time + edgeHopDelay;
        }

        // Smoothly slide to the calculated target lane
        Vector3 targetPos = new Vector3(
            levelData.centerPoint.x + levelData.EdgeOffsets[currentLane].x, 
            levelData.centerPoint.y + levelData.EdgeOffsets[currentLane].y, 
            transform.position.z // Lock Z to whatever Z it had when it hit the trigger
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * edgeMoveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, levelData.EdgeRotations[currentLane], Time.deltaTime * edgeMoveSpeed);
    }

    private void DecideNextLane()
    {
        if (playerScript == null) return;

        // Figure out adjacent lanes (wrapping around the polygon)
        int totalLanes = levelData.polygonSides;
        int leftLane = (currentLane - 1 + totalLanes) % totalLanes;
        int rightLane = (currentLane + 1) % totalLanes;

        // Get actual world positions of adjacent lanes
        Vector2 leftOffset = levelData.EdgeOffsets[leftLane];
        Vector2 rightOffset = levelData.EdgeOffsets[rightLane];
        
        Vector3 leftPos = new Vector3(levelData.centerPoint.x + leftOffset.x, levelData.centerPoint.y + leftOffset.y, transform.position.z);
        Vector3 rightPos = new Vector3(levelData.centerPoint.x + rightOffset.x, levelData.centerPoint.y + rightOffset.y, transform.position.z);

        // Which physical lane is closest to the player's physical ship?
        float distLeft = Vector3.Distance(leftPos, playerScript.transform.position);
        float distRight = Vector3.Distance(rightPos, playerScript.transform.position);

        // Move towards the closest side
        currentLane = (distLeft < distRight) ? leftLane : rightLane;
    }

    private void SnapToCurrentLane()
    {
        currentLane = Mathf.Clamp(currentLane, 0, levelData.polygonSides - 1);
        transform.position = new Vector3(
            levelData.centerPoint.x + levelData.EdgeOffsets[currentLane].x, 
            levelData.centerPoint.y + levelData.EdgeOffsets[currentLane].y, 
            transform.position.z
        );
        transform.rotation = levelData.EdgeRotations[currentLane];
    }

    private void OnTriggerEnter(Collider other)
    {

        // if enemy makes it to the edge, switch to edge walking mode and start hopping timer
        if (other.CompareTag("EdgeIndicator"))
        {
            currentState = EnemyState.OnEdge;
            nextHopTime = Time.time + edgeHopDelay; // Start hoping timer
            
            // Optionally snap its Z perfectly to the Rim's Z so it doesn't overshoot
            transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.position.z);
            SnapToCurrentLane(); // Re-snap just in case
        }
    }
}
