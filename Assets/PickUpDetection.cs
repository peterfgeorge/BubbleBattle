using UnityEngine;

public class PickUpDetection : MonoBehaviour
{
    public string itemName; // Set this in the Inspector or dynamically

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerController script from the collided player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Try to add the item to the player's inventory
                if (player.TryPickUpItem(itemName))
                {
                    Destroy(gameObject); // Destroy the item only if it was successfully picked up
                }
                else
                {
                    Debug.Log($"Player cannot pick up {itemName}. They already have an item.");
                }
            }
        }
    }
}

