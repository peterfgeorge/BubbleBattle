using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    public float acceleration = 5f;  // The rate of acceleration
    public float deceleration = 5f;
    private Vector2 moveDirection;
    private Vector2 currentVelocity;

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
        // If moving, gradually accelerate towards the target direction
        if (moveDirection != Vector2.zero)
        {
            // We calculate movement based on the direction the character is facing (transform.up)
            Vector2 forwardDirection = transform.up;  // 'Up' is the character's facing direction

            // Move towards the "top" of the character (forward)
            if (moveDirection.y > 0)  // Moving upwards in the control scheme
            {
                // Accelerate towards the forward direction
                currentVelocity = Vector2.Lerp(currentVelocity, forwardDirection * moveSpeed, acceleration * Time.fixedDeltaTime);
            }
            // Move away from the "top" of the character (backward)
            else if (moveDirection.y < 0)  // Moving downwards in the control scheme
            {
                // Accelerate away from the forward direction (negative velocity)
                currentVelocity = Vector2.Lerp(currentVelocity, -forwardDirection * moveSpeed, acceleration * Time.fixedDeltaTime);
            }

            // Handle rotation based on horizontal movement (left/right)
            if (moveDirection.x > 0)  // Moving right
            {
                // Rotate constantly to face right (0 degrees)
                transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime);  // Rotate counter-clockwise at a constant speed
            }
            else if (moveDirection.x < 0)  // Moving left
            {
                // Rotate constantly to face left (180 degrees)
                transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);  // Rotate clockwise at a constant speed
            }
        }
        else
        {
            // If no input, gradually decelerate to zero
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        // Apply the velocity to the rigidbody
        rb.linearVelocity = currentVelocity;
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

            float offsetDistance = transform.localScale.x * 2f;  // Multiply scale by a factor (adjust as necessary)
        
            if (moveDirection.x > 0)  // Player is moving right
            {
                spawnOffset = transform.right * offsetDistance;
            }
            else if (moveDirection.x < 0)  // Player is moving left
            {
                spawnOffset = -transform.right * offsetDistance;
            }
            else if (moveDirection.y > 0)  // Player is moving up
            {
                spawnOffset = transform.up * offsetDistance;
            }
            else if (moveDirection.y < 0)  // Player is moving down
            {
                spawnOffset = -transform.up * offsetDistance;
            }
            else  // Player is stationary, default to right
            {
                spawnOffset = transform.right * offsetDistance;
            }

            Vector3 spawnPosition = transform.position + spawnOffset;

            // Determine the type of projectile based on the current item
            Projectile.ProjectileType projectileType = currentItem == "Bomb" ? Projectile.ProjectileType.Bomb : Projectile.ProjectileType.Dart;

            // Instantiate the projectile at the adjusted spawn position
            GameObject proj = Instantiate(projectile, spawnPosition, transform.rotation);

            proj.AddComponent<Projectile>();

            Destroy(proj.GetComponent<ItemFunctions>());

            // Set the direction for the projectile based on moveDirection
            Vector2 fireDirection = moveDirection != Vector2.zero ? moveDirection : Vector2.right; // Default to right if stationary
            proj.GetComponent<Projectile>().SetupProjectile(fireDirection, gameObject, projectileType);

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


