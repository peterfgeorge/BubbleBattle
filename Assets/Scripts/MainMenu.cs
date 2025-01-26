using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public GameObject projectile;
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
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        
        if (playerManager == null) {
            Debug.LogError("No PlayerManager found!");
            return;
        }

        if (playerManager.ActivePlayersCount == 0) {
            Debug.LogWarning("No players have joined!");
            return;
        }

        playerManager.StartGame();

        // Get players from playerManager and run Death() on each
        foreach (GameObject player in playerManager.ActivePlayers) {
            if (!player.TryGetComponent<PlayerController>(out var pc)) {
                Debug.LogError("PlayerController not found on player!");
                return;
            }
            
            pc.Death();
        }
    }
}
