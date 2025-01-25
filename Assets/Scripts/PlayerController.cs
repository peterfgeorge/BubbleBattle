using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    private Vector2 moveDirection;

    public InputActionReference move;
    public InputActionReference fire;
    public GameObject projectile;

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

    // Modified Fire function to check for item in inventory and remove item after firing
    private void Fire(InputAction.CallbackContext obj)
    {
        if (currentItem != null)
        {
            Debug.Log($"Fired using item: {currentItem}");

            Vector3 spawnOffset;
            if (moveDirection.x > 0)  // Player is moving right
            {
                spawnOffset = transform.right * 1.0f;
            }
            else if (moveDirection.x < 0)  // Player is moving left
            {
                spawnOffset = -transform.right * 1.0f;
            }
            else if (moveDirection.y > 0)  // Player is moving up
            {
                spawnOffset = transform.up * 1.0f;
            }
            else if (moveDirection.y < 0)  // Player is moving down
            {
                spawnOffset = -transform.up * 1.0f;
            }
            else  // Player is stationary, default to right
            {
                spawnOffset = transform.right * 1.0f;
            }

            Vector3 spawnPosition = transform.position + spawnOffset;
            // Instantiate the projectile at the adjusted spawn position
            GameObject dart = Instantiate(projectile, spawnPosition, transform.rotation);

            // Set the direction for the projectile based on moveDirection
            Vector2 fireDirection = moveDirection != Vector2.zero ? moveDirection : Vector2.right; // Default to right if stationary
            dart.GetComponent<Projectile>().SetupProjectile(fireDirection, gameObject);

            // Remove the item from the inventory
            currentItem = null;
            Debug.Log("Item used and removed from inventory.");
        }
        else
        {
            Debug.Log("Cannot fire, no item in inventory.");
        }
    }



    // Function to handle inflation based on bubble count
    private void HandleInflation()
    {
        float inflationFactor = Mathf.Clamp01((float)bubbleCount / 10f);  // Adjust 10f for sensitivity
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

    public void LoseBubbles(int amount)
    {
        bubbleCount -= amount;
        Debug.Log($"Bubble count: {bubbleCount}");
    }
}


