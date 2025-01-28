using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    // An array of sprites to display when the game is over
    // Player1: [0], Player2: [1], Player3: [2], Player4: [3]
    [SerializeField]
    private GameObject[] _gameOverSprites;

    // Constant for restarting game
    private const int _restartTime = 3;

    // Show Winner Panel with the corresponding sprite from _gameOverSprites
    public void ShowWinner() {
        gameObject.SetActive(true);
        PlayerController winner = GetWinner();
        if (winner != null)
        {
            // Exit if the player index is out of bounds
            if (winner.PlayerIndex < 0 || winner.PlayerIndex > _gameOverSprites.Length) return;

            HideLosers(winner);

            _gameOverSprites[winner.PlayerIndex].SetActive(true);
        } else {
            Debug.LogWarning("No winner found!");
        }

        Debug.Log("RESTARTING GAME");
        Invoke(nameof(PlayAgain), _restartTime);
    }

    private void HideLosers(PlayerController winner) {
        foreach (GameObject player in GameDataManager.activePlayers)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != winner)
            {
                playerController.gameObject.SetActive(false);
            }
        }
    }
    
    private PlayerController GetWinner()
    {
        float highestBubbleCount = 0.0f;
        PlayerController winningPlayer = null;
        string scores = "Scores: ";

        foreach (GameObject player in GameDataManager.activePlayers)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            float bubbleCount = playerController.BubbleCount;

            scores += $"{playerController.name} - {bubbleCount}, ";

            if (bubbleCount > highestBubbleCount)
            {
                highestBubbleCount = bubbleCount;
                winningPlayer = playerController;
            }
        }

        if (winningPlayer != null)
        {
            Debug.Log($"Player {winningPlayer.name} wins with {highestBubbleCount} bubbles!");
        }
        
        Debug.Log(scores);

        return winningPlayer;
    }

    private void PlayAgain() {
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
            player.SetActive(true);

            if (!player.TryGetComponent<PlayerController>(out var pc)) {
                Debug.LogError("PlayerController not found on player!");
                return;
            }
            
            pc.Reset();
        }

        playerManager.StartGame();
    }
}