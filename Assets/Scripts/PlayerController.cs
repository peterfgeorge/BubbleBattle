using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    private Vector2 moveDirection;

    public InputActionReference move;
    public InputActionReference fire;

    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        Debug.Log($"Player {playerInput.playerIndex} using device: {playerInput.devices[0].displayName}");
    }
    // Inventory system for one item
    private string currentItem = null;

    // Bubble count (score) and inflation settings
    private int bubbleCount = 0;  // Tracks the number of bubbles
    public float inflationSpeed = 0.1f;  // How fast the player inflates
    private float maxScale = 2f;  // Max inflation scale (adjust as needed)

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        HandleInflation();  // Call to manage the inflation based on bubble count
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    private void OnEnable()
    {
        fire.action.started += Fire;
    }

    private void OnDisable()
    {
        fire.action.started -= Fire;
    }

    private void Fire(InputAction.CallbackContext obj)
    {
        Debug.Log("Fired!");
    }

    // Function to handle inflation based on bubble count
    private void HandleInflation()
    {
        // Calculate inflation factor based on bubble count
        float inflationFactor = Mathf.Clamp01((float)bubbleCount / 10f);  // Adjust 10f for sensitivity

        // Scale the player model based on the bubble count
        transform.localScale = Vector3.one + Vector3.one * inflationFactor * (maxScale - 1f);
    }

    // Function to pick up an item (Inventory check)
    public bool TryPickUpItem(string itemName)
    {
        if (currentItem == null)
        {
            currentItem = itemName;
            Debug.Log($"Picked up: {itemName}");
            return true;
        }

        Debug.Log("Inventory full!");
        return false;
    }

    // Function to add bubbles (increases bubble count)
    public void AddBubbles(int count)
    {
        bubbleCount += count;
        Debug.Log($"Bubble count: {bubbleCount}");

        // Optionally, cap the bubble count
        if (bubbleCount > 100)  // Max bubble count limit (adjustable)
        {
            bubbleCount = 100;
        }
    }

    // Optional function to drop the current item
    public void DropItem()
    {
        Debug.Log($"Dropped: {currentItem}");
        currentItem = null;
    }
}

