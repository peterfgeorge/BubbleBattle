using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Dart, Bomb, SeaWeed, SwordFish }

    public float speed = 10f;        // Speed of the dart
    public int damage = 1;           // Amount of bubbles to lose per hit
    
    public float rotationSpeed = 0.8f;

    private Vector2 direction;       // Direction the projectile will travel
    private GameObject shooter;      // Reference to the player that fired the projectile
    private ProjectileType projectileType = ProjectileType.Dart; // Default type is Dart

    private Transform player;        // Reference to the player for SwordFish rotation
    private float rotationAngle;     // Current angle of rotation for SwordFish
    private float rotationRadius = 2.0f; // Distance from the player for SwordFish

    public void SetupProjectile(Vector2 fireDirection, GameObject firingPlayer, ProjectileType type)
    {
        direction = fireDirection.normalized;
        shooter = firingPlayer;
        projectileType = type;
        player = firingPlayer.transform;

        if (projectileType == ProjectileType.SwordFish)
        {
            // Notify the PlayerController that the SwordFish is active
            firingPlayer.GetComponent<PlayerController>().SetSwordFishActive(true);
        }

        if (projectileType == ProjectileType.Bomb)
        {
            Destroy(gameObject, 6f);
        }
    }

    private void OnDestroy()
    {
        if (projectileType == ProjectileType.SwordFish && shooter != null)
        {
            // Notify the PlayerController that the SwordFish is no longer active
            shooter.GetComponent<PlayerController>().SetSwordFishActive(false);
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
        else if (projectileType == ProjectileType.SeaWeed)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        else if (projectileType == ProjectileType.SwordFish)
        {
            if (player != null)
            {
                // Rotate around the player
                rotationAngle += rotationSpeed * speed * Time.deltaTime; // Increment the angle over time
                float x = player.position.x + Mathf.Cos(rotationAngle) * rotationRadius;
                float y = player.position.y + Mathf.Sin(rotationAngle) * rotationRadius;

                transform.position = new Vector3(x, y, transform.position.z);
            }
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
                    Debug.Log("Bomb Hit");
                    player.LoseBubbles(damage);  // Apply damage by reducing bubbles
                    Destroy(gameObject);  // Destroy the bomb after triggering
                }
                else if (projectileType == ProjectileType.Dart)
                {
                    // Apply damage for the Dart type (if needed)
                    if (collision.gameObject == shooter) return;
                    Debug.Log("Dart Hit");
                    player.LoseBubbles(damage);
                    Destroy(gameObject);  // Destroy the dart after hitting the player
                }
                else if (projectileType == ProjectileType.SeaWeed)
                {
                    Debug.Log("SeaWeed Hit");
                    if (player != null)
                    {
                        player.ApplySlowEffect(0.5f, 3f); // Reduce speed to 50% for 3 seconds
                    }
                    Destroy(gameObject);
                }
                else if (projectileType == ProjectileType.SwordFish)
                {
                    // Apply damage for the Dart type (if needed)
                    Debug.Log("SwordFish Hit");
                    player.LoseBubbles(damage);
                    Destroy(gameObject);  // Destroy the dart after hitting the player
                }
            }
        }
    }
}



