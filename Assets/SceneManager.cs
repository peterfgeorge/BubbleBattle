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
        FindAndTeleportPlayerObjects();
    }

    private void FindAndTeleportPlayerObjects()
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        int playerIndex = 0; // For assigning spawn points

        foreach (GameObject obj in allObjects)
        {
            // Check if the object name contains "Player" (case-insensitive)
            if (obj.name.Contains("Player"))
            {
                // Get the spawn point (circular if more players than spawn points)
                Transform spawnPoint = spawnPoints.Length > playerIndex ? spawnPoints[playerIndex] : null;
                Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;

                // Teleport the player to the spawn point
                obj.transform.position = spawnPosition;
                Debug.Log($"Teleported {obj.name} to {spawnPosition}");

                playerIndex++;
            }
        }
    }
}
