using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Dart, Bomb }

    public float speed = 10f;        // Speed of the dart
    public int damage = 1;           // Amount of bubbles to lose per hit
    private Vector2 direction;       // Direction the projectile will travel
    private GameObject shooter;      // Reference to the player that fired the projectile
    private ProjectileType projectileType = ProjectileType.Dart; // Default type is Dart

    public void SetupProjectile(Vector2 fireDirection, GameObject firingPlayer, ProjectileType type)
    {
        direction = fireDirection.normalized;
        shooter = firingPlayer; // Store the player that fired the projectile
        projectileType = type;  // Set the type of projectile (Dart or Bomb)

        // If it's a bomb, start a timer to destroy it after 3 seconds
        if (projectileType == ProjectileType.Bomb)
        {
            Destroy(gameObject, 6f);  // Bomb will destroy itself after 6 seconds
        }
    }

    private void Update()
    {
        if (projectileType == ProjectileType.Dart)
        {
            // Only move if it's a Dart
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        // If it's a bomb, don't move, just stay in place
        else if (projectileType == ProjectileType.Bomb)
        {
            // Bomb stays stationary; you can add detonation logic here
        }
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
                // If the projectile is a Bomb, apply damage (or effect)
                if (projectileType == ProjectileType.Bomb)
                {
                    player.LoseBubbles(damage);  // Apply damage by reducing bubbles
                    Destroy(gameObject);  // Destroy the bomb after triggering
                }
                else if (projectileType == ProjectileType.Dart)
                {
                    // Apply damage for the Dart type (if needed)
                    player.LoseBubbles(damage);
                    Destroy(gameObject);  // Destroy the dart after hitting the player
                }
            }
        }
    }
}



