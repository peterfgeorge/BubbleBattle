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

        // Add BoxCollider2D and other necessary components to the bomb's GameObject
        BoxCollider2D boxCollider = bombInstance.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;  // Set the collider as trigger if needed

        // Optionally, add Rigidbody2D if necessary for physics-based interactions
        Rigidbody2D rb = bombInstance.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Call Detonate after a delay
        Invoke(nameof(Detonate), detonationTime);
    }

    private void Detonate()
    {
        // Logic for detonating the bomb, such as applying damage or effects
        Debug.Log("Bomb detonated!");

        // Destroy the bomb after detonation
        Destroy(bombInstance);
    }
}


