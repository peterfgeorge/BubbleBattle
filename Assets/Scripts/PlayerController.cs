using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    private Vector2 moveDirection;

    public InputActionReference move;
    public InputActionReference fire;

    // Single-slot inventory for carrying one item at a time
    private string currentItem = null;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
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

    // Function to pick up an item
    public bool TryPickUpItem(string itemName)
    {
        if (currentItem == null)
        {
            currentItem = itemName;
            Debug.Log($"Picked up: {itemName}");
            return true; // Item was successfully picked up
        }

        Debug.Log("Inventory full! Cannot pick up another item.");
        return false; // Inventory is full
    }

    // Function to check if the player has an item
    public bool HasItem()
    {
        return currentItem != null;
    }

    // Function to drop or clear the current item (optional, for future use)
    public void DropItem()
    {
        Debug.Log($"Dropped: {currentItem}");
        currentItem = null;
    }
}

