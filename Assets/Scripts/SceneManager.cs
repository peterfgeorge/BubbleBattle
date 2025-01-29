using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints; // Assign spawn points in the Inspector
    [SerializeField] private GameObject pauseCanvas;
    private bool isPaused = false;

    private void Start()
    {
        // Get all game objects that have been marked with DontDestroyOnLoad
        FindAndTeleportActivePlayers();
    }

    public void OnPause() {
        if (isPaused)
                ResumeGame();
            else
                PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale in case it's paused
        SceneManager.LoadScene("StartMenuScene");
    }

    private void FindAndTeleportActivePlayers()
    {
        // Find the "ActivePlayers" GameObject in the scene
        GameObject activePlayers = GameObject.Find("ActivePlayers");
        if (activePlayers == null)
        {
            Debug.LogError("No 'ActivePlayers' GameObject found in the scene!");
            return;
        }

        // Get the children of "ActivePlayers"
        Transform[] players = new Transform[activePlayers.transform.childCount];
        for (int i = 0; i < activePlayers.transform.childCount; i++)
        {
            players[i] = activePlayers.transform.GetChild(i);
        }

        // Create a list of available spawn points and shuffle it
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        ShuffleList(availableSpawnPoints);

        // Ensure enough spawn points are available
        if (availableSpawnPoints.Count < players.Length - 1) // Subtract 1 to exclude the parent
        {
            Debug.LogError("Not enough spawn points for all players!");
            return;
        }

        // Assign each player (excluding the parent) to a random unique spawn point
        int index = 0;
        foreach (Transform player in players)
        {
            if (player == activePlayers.transform) // Skip the parent GameObject
                continue;

            // Assign the player to a spawn point
            Transform spawnPoint = availableSpawnPoints[index];
            player.position = spawnPoint.position;
            Debug.Log($"Teleported {player.name} to {spawnPoint.position}");

            index++;
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
