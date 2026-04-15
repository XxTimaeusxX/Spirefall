using UnityEngine;
using UnityEngine.InputSystem;

public class Shipmovement : MonoBehaviour
{
    [Header("Level Reference")]
    public LevelGenerator levelData;

    [Header("Movement Settings")]
    public float snapSpeed = 15f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public float moveCooldown = 0.15f; 

    private int currentIndex = 0;
    private float nextMoveTime = 0f;

    void Start()
    {
        if (levelData == null) levelData = Object.FindFirstObjectByType<LevelGenerator>();

        if (levelData != null && levelData.EdgeOffsets != null && levelData.EdgeOffsets.Length > 0)
        {
            Vector3 startPos = new Vector3(levelData.centerPoint.x + levelData.EdgeOffsets[currentIndex].x, levelData.centerPoint.y + levelData.EdgeOffsets[currentIndex].y, transform.position.z);
            transform.position = startPos;
            transform.rotation = levelData.EdgeRotations[currentIndex];
        }
    }

    void Update()
    {
        if (levelData == null || levelData.EdgeOffsets == null) return;

        HandleInput();
        SmoothSnapTransform();
    }

    private void HandleInput()
    {
        if (moveAction == null || moveAction.action == null) return;
        
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        if (Time.time >= nextMoveTime)
        {
            if (moveInput.x > 0.5f)
            {
                MoveRight();
                nextMoveTime = Time.time + moveCooldown;
            }
            else if (moveInput.x < -0.5f)
            {
                MoveLeft();
                nextMoveTime = Time.time + moveCooldown;
            }
        }
    }

    private void MoveLeft()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = levelData.polygonSides - 1; 
        }
    }

    private void MoveRight()
    {
        currentIndex++;
        if (currentIndex >= levelData.polygonSides)
        {
            currentIndex = 0;
        }
    }

    private void SmoothSnapTransform()
    {
        Vector3 targetPos = new Vector3(
            levelData.centerPoint.x + levelData.EdgeOffsets[currentIndex].x, 
            levelData.centerPoint.y + levelData.EdgeOffsets[currentIndex].y, 
            transform.position.z 
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, levelData.EdgeRotations[currentIndex], Time.deltaTime * snapSpeed);
    }
}
