using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> players = new List<GameObject>();
    [SerializeField] List<GameObject> activePlayers = new List<GameObject>();
    [SerializeField] List<GameObject> unreadyPanels = new List<GameObject>();
    [SerializeField] List<GameObject> readyPanels = new List<GameObject>();
    private List<InputDevice> activeDevices = new List<InputDevice>();
    public string gameSceneName;

    private PlayerInputManager manager;
    private int index = 0;

    // Get activePlayers count with a getter
    public int ActivePlayersCount => activePlayers.Count;

    void Start()
    {
        manager = GetComponent<PlayerInputManager>();

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

    public void SwitchNextSpawnCharacter(PlayerInput input)
    {
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
    }

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
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
    }

    public void StartGame()
    {
        GameDataManager.activePlayers = activePlayers;
        SceneManager.LoadScene(gameSceneName);
    }
}
