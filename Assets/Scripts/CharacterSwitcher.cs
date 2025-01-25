using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{

    int index = 0;
    [SerializeField] List<GameObject> players = new List<GameObject>();
    PlayerInputManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponent<PlayerInputManager>();
        index = 0;
        manager.playerPrefab = players[index];
    }

    public void SwitchNextSpawnCharacter(PlayerInput input)
    {
        index += 1;
        manager.playerPrefab = players[index];
    }
}
