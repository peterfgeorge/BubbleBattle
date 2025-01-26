using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    // An array of sprites to display when the game is over
    // Player1: [0], Player2: [1], Player3: [2], Player4: [3]
    [SerializeField]
    private GameObject[] _gameOverSprites;

    // Show Winner Panel with the corresponding sprite from _gameOverSprites
    public void ShowWinner() {
        PlayerController winner = GetWinner();
        if (winner != null)
        {
            // Exit if the player index is out of bounds
            if (winner.PlayerIndex < 0 || winner.PlayerIndex > _gameOverSprites.Length) return;

            HideLosers(winner);
            
            _gameOverSprites[winner.PlayerIndex].SetActive(true);
        } else {
            Debug.LogError("No winner found!");
        }
    }

    public void HideLosers(PlayerController winner) {
        foreach (GameObject player in GameDataManager.activePlayers)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != winner)
            {
                playerController.gameObject.SetActive(false);
            }
        }
    }

    
    public PlayerController GetWinner()
    {
        float highestBubbleCount = 0.0f;
        PlayerController winningPlayer = null;

        foreach (GameObject player in GameDataManager.activePlayers)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            float bubbleCount = playerController.BubbleCount;

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
        return winningPlayer;
    }
}