using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Original grid spawning variables
    public GameObject cubePrefab;
    public float spacing = 1f;
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public int xoffset = 0;
    public int yoffset = 0;
    public int zoffset = 0;
    
    // New cursor spawning variables
    public GameObject[] spawnablePrefabs; // Array of different prefabs to spawn
    public float maxSpawnDistance = 10f;
    public LayerMask spawnLayerMask = -1; // What surfaces can be spawned on
    
    private int currentPrefabIndex = 0;
    private Camera playerCamera;
    
    public List<GameObject> spawned = new List<GameObject>();
    
    void Start()
    {
        // Get camera for cursor spawning
        playerCamera = Camera.main;
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();
            
        if (playerCamera == null)
        {
            Debug.LogError("No camera found! Cursor spawning won't work.");
        }
        
        // Original grid spawn on start
        spawnCubes();
        
        // Show current prefab info if we have prefabs assigned
        if (spawnablePrefabs != null && spawnablePrefabs.Length > 0)
            Debug.Log($"Current prefab: {spawnablePrefabs[currentPrefabIndex].name} ({currentPrefabIndex + 1}/{spawnablePrefabs.Length})");
    }

    void spawnCubes()
    {
        for (int i = 0; i < yoffset; i++)
        {
            for (int j = 0; j < zoffset; j++)
            {
                for (int k = 0; k < xoffset; k++)
                {
                    Vector3 position = new Vector3(k * spacing, i * spacing, j * spacing);
                    GameObject cubes = Instantiate(cubePrefab, position, Quaternion.identity, transform);
                    spawned.Add(cubes);
                }
            }
        }
    }

    void deleteCubes()
    {
        foreach (GameObject cube in spawned)
        {
            if (cube != null)
                Destroy(cube);
        }
        spawned.Clear();
    }
    
    // New cursor spawning methods
    void CyclePrefab(int direction)
    {
        if (spawnablePrefabs == null || spawnablePrefabs.Length == 0) return;
        
        currentPrefabIndex += direction;
        if (currentPrefabIndex >= spawnablePrefabs.Length)
            currentPrefabIndex = 0;
        else if (currentPrefabIndex < 0)
            currentPrefabIndex = spawnablePrefabs.Length - 1;
            
        Debug.Log($"Selected prefab: {spawnablePrefabs[currentPrefabIndex].name} ({currentPrefabIndex + 1}/{spawnablePrefabs.Length})");
    }
    
    void SpawnAtCursor()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("No camera available for cursor spawning!");
            return;
        }
        
        if (spawnablePrefabs == null || spawnablePrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned for cursor spawning!");
            return;
        }
        
        // Cast ray from camera through mouse position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Raycast with max distance and layer mask
        if (Physics.Raycast(ray, out hit, maxSpawnDistance, spawnLayerMask))
        {
            GameObject currentPrefab = spawnablePrefabs[currentPrefabIndex];
            if (currentPrefab != null)
            {
                GameObject spawnedObject = Instantiate(currentPrefab, hit.point, Quaternion.identity);
                spawned.Add(spawnedObject);
                Debug.Log($"Spawned {currentPrefab.name} at cursor position");
            }
        }
        else
        {
            Debug.Log("Cannot spawn here - no valid surface within range");
        }
    }

    void Update()
    {
        // Original controls
        if (Input.GetKeyDown(KeyCode.R))
        {
            deleteCubes();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnCubes();
        }
        
        // New cursor spawning controls
        if (Input.GetKeyDown(KeyCode.Q)) // Previous prefab
        {
            CyclePrefab(-1);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Next prefab
        {
            CyclePrefab(1);
        }
        
        // Direct prefab selection with number keys 1-9
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (spawnablePrefabs != null && i - 1 < spawnablePrefabs.Length)
                {
                    currentPrefabIndex = i - 1;
                    Debug.Log($"Selected prefab: {spawnablePrefabs[currentPrefabIndex].name} ({currentPrefabIndex + 1}/{spawnablePrefabs.Length})");
                }
            }
        }
        
        // Spawn at cursor with left click
        if (Input.GetMouseButtonDown(0))
        {
            SpawnAtCursor();
        }
    }
}