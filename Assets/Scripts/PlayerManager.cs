using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> players = new List<GameObject>();
    [SerializeField] public List<GameObject> activePlayers = new List<GameObject>();
    [SerializeField] List<GameObject> unreadyPanels = new List<GameObject>();
    [SerializeField] List<GameObject> readyPanels = new List<GameObject>();
    [SerializeField] public string allowedSceneName = "StartMenuScene";
    private List<InputDevice> activeDevices = new List<InputDevice>();
    public string[] gameSceneName;
    public int[] playerScores = new int[4];
    public int playerIndexWhoWon;

    private PlayerInputManager manager;
    private int index = 0;

    // Get activePlayers count with a getter
    public int ActivePlayersCount => activePlayers.Count;

    public List<GameObject> ActivePlayers => activePlayers;

    public GameObject activePlayersParent;

    private static PlayerManager instance;

    [SerializeField]
    private Button startButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates
        }
    }

    void Start()
    {
        manager = GetComponent<PlayerInputManager>();
        DontDestroyOnLoad(manager);
        DontDestroyOnLoad(activePlayersParent);

        if (players.Count == 0)
        {
            Debug.LogError("No players assigned to CharacterSwitcher!");
            return;
        }

        index = 0;
        // manager.playerPrefab = players[index];

        // Subscribe to player join event
        manager.onPlayerJoined += OnPlayerJoined;
    }

    public void SwitchNextSpawnCharacter(PlayerInput input) // where is this used?
    {
        Debug.Log("This dumbass script is running!!!!");
        if (players.Count == 0)
        {
            Debug.LogError("No player prefabs available!");
            return;
        }

        if (index >= unreadyPanels.Count)
        {
            Debug.LogWarning("Maximum number of players reached!");
            return;
        }

        if (SceneManager.GetActiveScene().name != allowedSceneName)
        {
            Debug.Log($"Player join attempt blocked. Current scene: {SceneManager.GetActiveScene().name}");
            return;
        }
        
        // Activate the corresponding unready panel
        unreadyPanels[index].SetActive(false);
        readyPanels[index].SetActive(true);
        
        Transform artTransform = players[index].transform.Find("Art");
        SpriteRenderer artSpriteRenderer = artTransform.GetComponent<SpriteRenderer>();
        Image panelImage = readyPanels[index].transform.Find("Image")?.GetComponent<Image>();
        panelImage.sprite = artSpriteRenderer.sprite;

        // Increment index and wrap around
        index = (index + 1) % players.Count;

        // Update the player prefab for the next spawn
        manager.playerPrefab = players[index];
        Debug.Log($"Next spawn set to: {players[index].name}");

        startButton.interactable = index > 0;
    }

    private List<GameObject> FindPanelsInHierarchy(Transform parent, string tag)
    {
        List<GameObject> panelList = new List<GameObject>();

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true)) // true includes inactive objects
        {
            if (child.CompareTag(tag))
            {
                panelList.Add(child.gameObject);
            }
        }

        return panelList;
    }

    public void GetPlayersWhoAlreadyJoined() {
        Transform canvas = GameObject.Find("Canvas")?.transform;
        Transform readyUpMenu = canvas?.Find("ReadyUpMenu");

        unreadyPanels = FindPanelsInHierarchy(readyUpMenu, "UnreadyPanel");
        readyPanels = FindPanelsInHierarchy(readyUpMenu, "ReadyPanel");

        // Get the number of children (players) under activePlayersParent
        int childCount = activePlayersParent.transform.childCount;
        Debug.Log($"Total Players in Scene: {childCount}");

        for (int i = 0; i < childCount; i++)
        {
            Transform playerTransform = activePlayersParent.transform.GetChild(i);
            Debug.Log($"Player index {i} is already joined");

            // Activate the corresponding unready panel
            unreadyPanels[i].SetActive(false);
            readyPanels[i].SetActive(true);

            // Get the player's sprite from the "Art" child object
            Transform artTransform = playerTransform.Find("Art");
            if (artTransform != null)
            {
                SpriteRenderer artSpriteRenderer = artTransform.GetComponent<SpriteRenderer>();
                Image panelImage = readyPanels[i].transform.Find("Image")?.GetComponent<Image>();

                if (panelImage != null && artSpriteRenderer != null)
                {
                    panelImage.sprite = artSpriteRenderer.sprite;
                }
                else
                {
                    Debug.LogWarning($"Missing components on player {playerTransform.name} at index {i}");
                }
            }
            else
            {
                Debug.LogWarning($"Art transform not found on player {playerTransform.name} at index {i}");
            }
        }
    }

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        if (SceneManager.GetActiveScene().name != allowedSceneName)
        {
            Debug.Log($"Player join attempt blocked. Current scene: {SceneManager.GetActiveScene().name}");
            Destroy(newPlayer.gameObject); // Prevent the player from spawning
            return;
        }

        // Get the device used to spawn this player
        InputDevice spawningDevice = newPlayer.devices[0];

        // Log the device for debugging
        Debug.Log($"Player {newPlayer.playerIndex} joined using device: {spawningDevice.displayName}");

        // Add the new player to the list of active players
        activePlayers.Add(newPlayer.gameObject);
        DontDestroyOnLoad(newPlayer);

        // Explicitly assign the control scheme to lock the player to their device
        newPlayer.SwitchCurrentControlScheme(spawningDevice);

        // Optionally assign a custom name to the player
        newPlayer.gameObject.name = $"Player {newPlayer.playerIndex}";

        newPlayer.gameObject.transform.SetParent(activePlayersParent.transform);
        Debug.Log("Active player count " + activePlayers.Count);
    }

    public void GoToWinScreen()
    {
        GameDataManager.activePlayers = activePlayers;

        activePlayers.ForEach(player => player.SetActive(true));
        Debug.Log("players should be active");

        int playerCount = activePlayers.Count;

        switch (playerCount)
        {
            case 2:
                SceneManager.LoadScene("2PlayerRacetrack");
                break;
            case 3:
                SceneManager.LoadScene("3PlayerRacetrack");
                break;
            case 4:
                SceneManager.LoadScene("4PlayerRacetrack");
                break;
            default:
                Debug.LogError("Invalid player count: " + playerCount);
                break;
        }
    }

    public void StartGame()
    {
        GameDataManager.activePlayers = activePlayers;

        activePlayers.ForEach(player => player.SetActive(true));

        // Get a random index from the gameSceneName array
        int randomIndex = Random.Range(0, gameSceneName.Length);

        // Load the random scene
        SceneManager.LoadScene(gameSceneName[randomIndex]);
    }
}
