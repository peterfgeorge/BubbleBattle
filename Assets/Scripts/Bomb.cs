using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bomb : MonoBehaviour
{
    [SerializeField]
    private int _projectiles = 6;

    [SerializeField]
    private float _detonationTime = 3.0f;

    [SerializeField]
    private int _projectileSpeed = 200;
    [SerializeField]
    private float _projectileScale = 0.5f;
    [SerializeField]
    private Sprite _projectileSprite;

    private float _radius = 0;
    private float _arcStride = 0;

    void Awake() 
    {
        _radius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        _arcStride = 360.0f / _projectiles;
    }

    void Start() 
    {
        Invoke(nameof(Detonate), _detonationTime);
    }

    private void Detonate() 
    {
        for (int i = 0; i < _projectiles; i++)
        {
            GameObject projectile = new GameObject("Projectile");
            projectile.transform.SetParent(transform);

            float angle = i * Mathf.PI * 2 / _projectiles;
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);

            projectile.AddComponent<Rotator>();

            // TODO: Fix the rotation of the projectiles
            // Debug.Log("Arc Stride: " + (i * _arcStride) + " xAngle: " + xAngle + " yAngle: " + yAngle);
            // Set the position along the circumference of the circle based on the arc stride
            // projectile.transform.SetLocalPositionAndRotation(
            //     new Vector3(
            //         xAngle * _radius, 
            //         yAngle * _radius,
            //     0
            //     ),
            //     Quaternion.Euler(0, 0, i * _arcStride + 45)
            // );

            projectile.transform.localPosition = new Vector3(xAngle * _radius, yAngle * _radius, 0);
            projectile.transform.localScale = new Vector3(_projectileScale, _projectileScale, 1);

            SpriteRenderer projectileSpriteRenderer = projectile.AddComponent<SpriteRenderer>();
            projectileSpriteRenderer.sprite = _projectileSprite;
            projectileSpriteRenderer.sortingOrder = 1;

            BoxCollider2D boxCollider = projectile.AddComponent<BoxCollider2D>();
            float projectileSize = projectileSpriteRenderer.bounds.size.x;
            boxCollider.size = new Vector2(projectileSize, projectileSize);
            boxCollider.isTrigger = true;

            Rigidbody2D rigidbody = projectile.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;

            boxCollider.AddComponent<Projectile>();

            // Revisit later: Keeping because it caused an effect that could be exaggerated
            // rigidbody.linearVelocity = _projectileSpeed * Time.deltaTime * projectile.transform.up;
            
            // Move the rigid body in the direction of the angle
            rigidbody.linearVelocity = new Vector2(
                xAngle * _projectileSpeed * Time.deltaTime,
                yAngle * _projectileSpeed * Time.deltaTime
            );
        }
    }
}
