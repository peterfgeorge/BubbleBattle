using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;        // Speed of the dart
    public int damage = 1;          // Amount of bubbles to lose per hit
    private Vector2 direction;      // Direction the dart will travel
    private GameObject shooter;     // Reference to the player that fired the dart

    public void SetupProjectile(Vector2 fireDirection, GameObject firingPlayer)
    {
        direction = fireDirection.normalized;
        shooter = firingPlayer; // Store the player that fired the dart
    }

    private void Update()
    {
        // Move the dart in the set direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collision with the player who fired the projectile
        if (collision.gameObject == shooter) return;
        Debug.Log("Projectile collided with: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            // Get the PlayerController script from the collided player
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Apply damage by reducing bubbles
                player.LoseBubbles(damage);

                // Destroy the dart after hitting a player
                Destroy(gameObject);
            }
        }
    }
}


