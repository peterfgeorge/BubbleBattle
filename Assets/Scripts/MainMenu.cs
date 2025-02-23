using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    // public void PlayGame()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    // }
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void PlayAgain() {
        Debug.Log("Play again called");
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        
        if (playerManager == null) {
            Debug.LogError("No PlayerManager found!");
            return;
        }

        if (playerManager.ActivePlayersCount == 0) {
            Debug.LogWarning("No players have joined!");
            return;
        }

        // Get players from playerManager and run Death() on each
        foreach (GameObject player in playerManager.ActivePlayers) {
            if (!player.TryGetComponent<PlayerController>(out var pc)) {
                Debug.LogError("PlayerController not found on player!");
                return;
            }
            
            pc.Death();
        }

        playerManager.StartGame();
    }
}
