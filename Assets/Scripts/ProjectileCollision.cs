using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponents<ProjectileCollision>().Length > 0) {
            return;
        }
        
        Destroy(gameObject);
    }
}
