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
        var gameSceneName = GameDataManager.playAgainGameSceneName;
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("No game scene name assigned to GameDataManager!");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}
