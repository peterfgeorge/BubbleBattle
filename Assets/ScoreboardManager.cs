using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScoreboardManager : MonoBehaviour
{
    public List<GameObject> spriteList = new List<GameObject>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    private List<GameObject> spawnedObjects = new List<GameObject>(); // Store spawned GameObjects
    private GameObject gameManager;
    private PlayerManager playerManager;
    private int highestScore;
    private int playerIndexWhoWon;

    void Start()
    {
        gameManager = GameObject.Find("GameManager");

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
            return;
        }

        playerManager = gameManager.GetComponent<PlayerManager>();

        if (playerManager == null)
        {
            Debug.LogError("PlayerManager component not found on GameManager!");
            return;
        }

        Debug.Log("spawn sprites ab to be called");
        SpawnSprites();
        int highestScoreIndex = GetHighestScoreIndex();

        if (playerManager.playerIndexWhoWon != -1) {
            playerIndexWhoWon = playerManager.playerIndexWhoWon;
        } else {
            playerIndexWhoWon = 0;
        }
        StartCoroutine(MoveWinnerSpriteUp(spawnedObjects[playerIndexWhoWon], 1.46f, 1f));
    }

    private void SpawnSprites()
    {
        spawnedObjects.Clear(); // Clear previous list before spawning new objects
        int count = Mathf.Min(spriteList.Count, spawnPoints.Count, playerManager.playerScores.Length);

        for (int i = 0; i < count; i++)
        {
            if (spriteList[i] != null && spawnPoints[i] != null)
            {
                Debug.Log("hiiii");
                // Get player's current score
                float playerScore = playerManager.playerScores[i];
                float newYPosition;

                // Calculate new y-position
                if(i == playerManager.playerIndexWhoWon) {
                    newYPosition = spawnPoints[i].transform.position.y + (playerScore * 1.46f) - 1.46f;
                } else {
                    newYPosition = spawnPoints[i].transform.position.y + (playerScore * 1.46f);
                }
                

                // Set new spawn position with updated Y value
                Vector3 spawnPosition = new Vector3(
                    spawnPoints[i].transform.position.x,
                    newYPosition,
                    spawnPoints[i].transform.position.z
                );

                // Instantiate the sprite at the calculated position
                GameObject spawnedSprite = Instantiate(spriteList[i], spawnPosition, Quaternion.identity);
                spawnedObjects.Add(spawnedSprite); // Store the spawned GameObject
            }
        }
        Debug.Log("finished spawning sprites");
        // yield return new WaitForSeconds(2f);
    }

    private int GetHighestScoreIndex()
    {
        if (playerManager.playerScores == null || playerManager.playerScores.Length == 0)
        {
            return -1; // Return -1 if no scores exist
        }

        int highestIndex = -1;
        highestScore = playerManager.playerScores[0];

        for (int i = 1; i < playerManager.playerScores.Length; i++)
        {
            if (playerManager.playerScores[i] > highestScore)
            {
                highestScore = playerManager.playerScores[i];
                highestIndex = i;
            }
        }
        Debug.Log("got highest score");
        return highestIndex;
    }

    private IEnumerator MoveWinnerSpriteUp(GameObject winner, float distance, float duration)
    {
        yield return new WaitForSeconds(1f);

        if(playerManager.playerIndexWhoWon != -1) {
            if (winner == null) yield break;

            Vector3 startPosition = winner.transform.position;
            Vector3 targetPosition = startPosition + new Vector3(0, distance, 0);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                winner.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            winner.transform.position = targetPosition; // Ensure it reaches the exact position
        }

        yield return new WaitForSeconds(1f);

        if (highestScore > 4) {
            Debug.Log("going to start menu");
            SceneManager.LoadScene("StartMenuScene");
        } else {
            Debug.Log("going again");
            playerManager.StartGame();
        }
    }
}
