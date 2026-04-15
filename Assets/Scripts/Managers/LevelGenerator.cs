using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Polygon Settings")]
    public int polygonSides = 5; 
    public float radius = 5f;
    public Vector2 centerPoint = Vector2.zero;
    public float angleOffset = -90f;

    [Header("Visual Settings")]
    public float tubeLength = 50f;
    
    [Tooltip("Material used for the glowing lines (e.g., standard lit or unlit blue)")]
    public Material lineMaterial;
    public float lineWidth = 0.1f;

    public Vector2[] EdgeOffsets { get; private set; }
    public Quaternion[] EdgeRotations { get; private set; }

    // Store generated lines so we can clear them if we regenerate
    private List<GameObject> generatedLines = new List<GameObject>();

    void Awake()
    {
        GeneratePolygonPoints();
    }

    void Start()
    {
        // Build the physical lines when the game starts
        BuildVisualGrid();
    }

    public void GeneratePolygonPoints()
    {
        if (polygonSides < 3) polygonSides = 3;

        EdgeOffsets = new Vector2[polygonSides];
        EdgeRotations = new Quaternion[polygonSides];

        float angleStep = 360f / polygonSides;

        for (int i = 0; i < polygonSides; i++)
        {
            float currentAngle = (i * angleStep) + angleOffset;
            float angleInRadians = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;
            
            EdgeOffsets[i] = new Vector2(x, y);

            Vector2 directionToCenter = -EdgeOffsets[i].normalized; 
            float rotationZ = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
            
            EdgeRotations[i] = Quaternion.Euler(0, 0, rotationZ + 90f);
        }
    }

  
    public void BuildVisualGrid()
    {
        // Clear old lines if regenerating
        foreach (GameObject line in generatedLines) Destroy(line);
        generatedLines.Clear();

        if (polygonSides < 3) return;

        float angleStep = 360f / polygonSides;
        Vector3[] frontCorners = new Vector3[polygonSides];
        Vector3[] backCorners = new Vector3[polygonSides];
        float frontZ = transform.position.z; 
        float backZ = transform.position.z + tubeLength;
        float cornerRadius = radius / Mathf.Cos((180f / polygonSides) * Mathf.Deg2Rad);

        // Precalculate corners
        for (int i = 0; i < polygonSides; i++)
        {
            float cornerAngle = (i * angleStep) + angleOffset - (angleStep / 2f);
            float cornerRad = cornerAngle * Mathf.Deg2Rad;
            float cornerX = centerPoint.x + Mathf.Cos(cornerRad) * cornerRadius;
            float cornerY = centerPoint.y + Mathf.Sin(cornerRad) * cornerRadius;

            frontCorners[i] = new Vector3(cornerX, cornerY, frontZ);
            backCorners[i] = new Vector3(cornerX, cornerY, backZ);
        }

        // 1. Create Front Ring
        CreateLineLoop("Front Ring", frontCorners);

        // 2. Create Back Ring
        CreateLineLoop("Back Ring", backCorners);

        // 3. Create Spokes (Tunnel Lines)
        for (int i = 0; i < polygonSides; i++)
        {
            CreateLineSegment($"Spoke {i}", frontCorners[i], backCorners[i]);
        }
    }

    private void CreateLineLoop(string name, Vector3[] points)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform); // parent to LevelGenerator
        generatedLines.Add(lineObj);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        ConfigureLineRenderer(lr);

        lr.loop = true; // Connects the last point back to the first point!
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }

    private void CreateLineSegment(string name, Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.SetParent(transform);
        generatedLines.Add(lineObj);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        ConfigureLineRenderer(lr);

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    private void ConfigureLineRenderer(LineRenderer Linerenderer)
    {
        if (lineMaterial != null) Linerenderer.material = lineMaterial;
        else Linerenderer.material = new Material(Shader.Find("Sprites/Default")); // Generic unlit fallback

        Linerenderer.startWidth = lineWidth;
        Linerenderer.endWidth = lineWidth;
        Linerenderer.useWorldSpace = true;
        Linerenderer.generateLightingData = false;
        Linerenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Linerenderer.receiveShadows = false;
    }

    // Keep the Gizmos so you can still see it in the editor while the game is stopped
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
        {
            GeneratePolygonPoints();
        }

        if (polygonSides < 3) return;

        float angleStep = 360f / polygonSides;
        
        // Setup arrays for front and back corners
        Vector3[] frontCorners = new Vector3[polygonSides];
        Vector3[] backCorners = new Vector3[polygonSides];
        
        // Z depths
        float frontZ = transform.position.z; 
        float backZ = transform.position.z + tubeLength;

        // Calculate corners (vertices)
        float cornerRadius = radius / Mathf.Cos((180f / polygonSides) * Mathf.Deg2Rad);

        for (int i = 0; i < polygonSides; i++)
        {
            float cornerAngle = (i * angleStep) + angleOffset - (angleStep / 2f);
            float cornerRad = cornerAngle * Mathf.Deg2Rad;
            
            float cornerX = centerPoint.x + Mathf.Cos(cornerRad) * cornerRadius;
            float cornerY = centerPoint.y + Mathf.Sin(cornerRad) * cornerRadius;

            frontCorners[i] = new Vector3(cornerX, cornerY, frontZ);
            backCorners[i] = new Vector3(cornerX, cornerY, backZ);
        }

        // Draw the rings and the tunnel segments
        Gizmos.color = Color.green;
        for (int i = 0; i < polygonSides; i++)
        {
            int nextIndex = (i + 1) % polygonSides;

            // Draw FRONT polygon ring
            Gizmos.DrawLine(frontCorners[i], frontCorners[nextIndex]);
            
            // Draw BACK polygon ring
            Gizmos.DrawLine(backCorners[i], backCorners[nextIndex]);
            
            // Draw CONNECTING lines down the tube (the corners)
            Gizmos.DrawLine(frontCorners[i], backCorners[i]);
        }

        // Draw cyan spheres where the ship/enemies snap at the front
        Gizmos.color = Color.cyan;
        for (int i = 0; i < polygonSides; i++)
        {
            float edgeAngle = ((i * angleStep) + angleOffset) * Mathf.Deg2Rad;
            float edgeX = centerPoint.x + Mathf.Cos(edgeAngle) * radius;
            float edgeY = centerPoint.y + Mathf.Sin(edgeAngle) * radius;

            Vector3 frontEdgeCenter = new Vector3(edgeX, edgeY, frontZ);
            Gizmos.DrawSphere(frontEdgeCenter, 0.2f);
        }
    }
}
