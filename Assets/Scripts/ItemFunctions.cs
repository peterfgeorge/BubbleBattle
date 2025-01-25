using UnityEngine;

public class ItemFunctions : MonoBehaviour
{
    public string itemName; // Set this in the Inspector or dynamically
    public bool isBubbleItem = false;  // Flag to check if it's a bubbles item

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get the PlayerController script from the collided player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                if (isBubbleItem)
                {
                    // If it's a Bubbles item, increase the bubble count
                    player.AddBubbles(1);  // Add 1 bubble (adjustable)
                    Destroy(gameObject);   // Destroy the bubbles item after pickup
                }
                else
                {
                    // Regular item pickup (inventory)
                    if (player.TryPickUpItem(itemName))
                    {
                        player.projectile = Resources.Load("Prefabs/" + itemName, typeof(GameObject)) as GameObject;
                        Destroy(gameObject);  // Destroy the regular item after pickup
                    }
                    else
                    {
                        Debug.Log($"Player cannot pick up {itemName}. Inventory is full.");
                    }
                }
            }
        }
    }
}


