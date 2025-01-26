using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float rotationSpeed = 200f;

    public float acceleration = 5f;  // The rate of acceleration
    public float deceleration = 5f;
    private Vector2 moveInput;  // Stores input from left stick
    private Vector2 rotateInput;  // Stores input from right stick
    private Vector2 currentVelocity;

    public InputActionReference move;
    public InputActionReference fire;
    public GameObject projectile;

    private bool isSlowed = false;


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
        moveInput = move.action.ReadValue<Vector2>();
        HandleInflation();  // Call to manage the inflation based on bubble count
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Read input from the left stick (Y-axis for forward/backward movement)
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        // Read input from the right stick (X-axis for rotation)
        rotateInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Handle movement (forward/backward)
        Vector2 forwardMovement = transform.up * moveInput.y * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // Handle rotation (left/right)
        float rotationAmount = -rotateInput.x * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotationAmount);
    }


    public void ApplySlowEffect(float speedMultiplier, float duration)
    {
        if (isSlowed) return; // Avoid stacking slows

        isSlowed = true;
        float originalSpeed = moveSpeed;
        moveSpeed *= speedMultiplier; // Reduce speed by multiplier
        Debug.Log($"Player slowed! Speed reduced to {moveSpeed} for {duration} seconds.");

        // Restore the original speed after the duration
        StartCoroutine(RemoveSlowEffect(originalSpeed, duration));
    }

    private System.Collections.IEnumerator RemoveSlowEffect(float originalSpeed, float duration)
    {
        yield return new WaitForSeconds(duration); // Wait for the slow duration
        moveSpeed = originalSpeed; // Restore the original speed
        isSlowed = false; // Allow future slows
        Debug.Log("Player speed restored.");
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
        
            if (moveInput.x > 0)  // Player is moving right
            {
                spawnOffset = transform.right * offsetDistance;
            }
            else if (moveInput.x < 0)  // Player is moving left
            {
                spawnOffset = -transform.right * offsetDistance;
            }
            else if (moveInput.y > 0)  // Player is moving up
            {
                spawnOffset = transform.up * offsetDistance;
            }
            else if (moveInput.y < 0)  // Player is moving down
            {
                spawnOffset = -transform.up * offsetDistance;
            }
            else  // Player is stationary, default to right
            {
                spawnOffset = transform.right * offsetDistance;
            }

            Vector3 spawnPosition = transform.position + spawnOffset;

            // Determine the type of projectile based on the current item
            Projectile.ProjectileType projectileType;

            Debug.Log("CURRENT ITEM: " + currentItem);
            if (currentItem == "Bomb")
            {
                projectileType = Projectile.ProjectileType.Bomb;
            }
            else if(currentItem == "SeaWeed")
            {
                projectileType = Projectile.ProjectileType.SeaWeed;
            }
            else
            {
                projectileType = Projectile.ProjectileType.Dart;
            }

            // Instantiate the projectile at the adjusted spawn position
            GameObject proj = Instantiate(projectile, spawnPosition, transform.rotation);

            proj.AddComponent<Projectile>();

            Destroy(proj.GetComponent<ItemFunctions>());

            // Set the direction for the projectile based on moveInput
            Vector2 fireDirection = moveInput != Vector2.zero ? moveInput : Vector2.right; // Default to right if stationary
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


