using UnityEngine;

public class ExistingPlayers : MonoBehaviour
{
    public void GetExistingPlayers()
    {
        // Find the GameManager in the scene
        PlayerManager gameManager = GameObject.Find("GameManager")?.GetComponent<PlayerManager>();

        // Check if GameManager is found before calling the method
        if (gameManager != null)
        {
            gameManager.GetPlayersWhoAlreadyJoined();
        }
        else
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }
    public void StartGame() {
        // Find the GameManager in the scene
        PlayerManager gameManager = GameObject.Find("GameManager")?.GetComponent<PlayerManager>();

        // Check if GameManager is found before calling the method
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        else
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }
}
