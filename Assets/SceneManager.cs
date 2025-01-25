using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; // Assign spawn points in the Inspector

    private void Start()
    {
        // Get all game objects that have been marked with DontDestroyOnLoad
        GameObject[] dontDestroyObjects = FindDontDestroyOnLoadObjects();

        if (dontDestroyObjects.Length == 0)
        {
            Debug.LogWarning("No DontDestroyOnLoad objects found.");
            return;
        }

        // Teleport each object to a spawn point
        for (int i = 0; i < dontDestroyObjects.Length; i++)
        {
            GameObject playerObject = dontDestroyObjects[i];

            // Check if a spawn point exists for the index
            if (spawnPoints.Length > i)
            {
                Transform spawnPoint = spawnPoints[i];
                playerObject.transform.position = spawnPoint.position;
                Debug.Log($"Teleported {playerObject.name} to spawn point {i} at {spawnPoint.position}");
            }
            else
            {
                Debug.LogWarning($"No spawn point found for player {i}. Skipping teleportation.");
            }
        }
    }

    // Find all objects that have been marked with DontDestroyOnLoad
    private GameObject[] FindDontDestroyOnLoadObjects()
    {
        // This will get all objects that are marked as "DontDestroyOnLoad"
        List<GameObject> dontDestroyObjects = new List<GameObject>();

        // Get all objects of type GameObject in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if the object is marked with DontDestroyOnLoad
            if (obj.scene.name == null)
            {
                dontDestroyObjects.Add(obj);
            }
        }

        return dontDestroyObjects.ToArray();
    }
}
