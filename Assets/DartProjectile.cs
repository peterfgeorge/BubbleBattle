using UnityEngine;

public class DartProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10; // Damage dealt to the player
    public float lifespan = 5f; // Dart lifetime before it disappears

    private Vector2 direction;

    // Set up the Dart projectile with a direction (typically from the player)
    public void SetupDart(Vector2 fireDirection)
    {
        direction = fireDirection;
        Destroy(gameObject, lifespan);  // Destroy the Dart after its lifespan
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);  // Move the Dart
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Get PlayerController and apply damage (and remove bubbles)
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddBubbles(-damage);  // Remove bubbles when hit
                Destroy(gameObject);  // Destroy the Dart projectile on collision
            }
        }
    }
}
