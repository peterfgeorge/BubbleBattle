using System;
using UnityEngine;

public class PlayerReady : MonoBehaviour
{
    public GameObject playerPrefab;

    public PlayerManager playerManager;
    public bool isReady = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isReady)
        {
            // Start the game
        }
    }

    // private bool toggleReadyUp()
    // {
    //     isReady = !isReady;
    // }
}
