using UnityEngine;

public class BombItem : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab;

    [SerializeField]
    private float detonationTime = 3f;

    private GameObject bombInstance;

    public void FireBomb()
    {
        // Instantiate the bomb at the player's current position
        bombInstance = Instantiate(bombPrefab, transform.position, Quaternion.identity);

        // Add necessary components to the bomb's GameObject (e.g., Collider2D)
        BoxCollider2D boxCollider = bombInstance.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;  // Set the collider as trigger to detect player collisions

        // Optionally, add Rigidbody2D if necessary for physics-based interactions
        Rigidbody2D rb = bombInstance.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;  // Prevent bomb from falling due to gravity

        // Bomb will be handled by Projectile.cs now for destruction and collision
    }

    private void Detonate()
    {
        // Logic for detonating the bomb, such as applying damage or effects
        Debug.Log("Bomb detonated!");

        // Destroy the bomb after detonation
        Destroy(bombInstance);
    }
}


