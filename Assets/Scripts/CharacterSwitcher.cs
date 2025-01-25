using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    [SerializeField] List<GameObject> players = new List<GameObject>();
    private PlayerInputManager manager;
    private int index = 0;

    void Start()
    {
        manager = GetComponent<PlayerInputManager>();

        if (players.Count == 0)
        {
            Debug.LogError("No players assigned to CharacterSwitcher!");
            return;
        }

        index = 0;
        manager.playerPrefab = players[index];

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

        // Explicitly assign the control scheme to lock the player to their device
        newPlayer.SwitchCurrentControlScheme(spawningDevice);

        // Optionally assign a custom name to the player
        newPlayer.gameObject.name = $"Player {newPlayer.playerIndex}";
    }
}
