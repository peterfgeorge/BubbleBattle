using System;
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

    [SerializeField]
    private bool _destroyOnDetonate = false;

    private float _radius;
    private float _timeUntilDetonation;

    void Awake() 
    {
        float localScale = transform.localScale.x;
        float diameter = GetComponent<SpriteRenderer>().bounds.size.x  * Mathf.Sign(localScale);
        _radius =  diameter / 2;

        Debug.Log("Bomb localScale: " + localScale + " radius: " + _radius);
    }

    void Start()
    {
        // Resets detonation time when the bomb is re-enabled
        _timeUntilDetonation = _detonationTime;
        Debug.Log("Bomb detonation time: " + _detonationTime + " seconds");
    }

    void Update()
    {
        if (_timeUntilDetonation <= 0)
        {
            Debug.Log("Bomb detonated! Projectiles: " + _projectiles);
            Debug.Log("transform position: " + transform.position);
            Detonate(transform);
            _timeUntilDetonation = _detonationTime;

            if (_destroyOnDetonate) 
            {
                Destroy(gameObject);
            }
        } 
        else 
        {
            _timeUntilDetonation -= Time.deltaTime;
        }

        DrawDebugCircle(transform.position, _radius, _projectiles, Color.red);
    }

    private void Detonate(Transform startPosition) 
    {
        for (int i = 0; i < _projectiles; i++)
        {
            GameObject projectile = new GameObject("Projectile");

            float angle = i * Mathf.PI * 2 / _projectiles;
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);

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

            projectile.transform.position = new Vector3(
                xAngle * _radius + startPosition.position.x, 
                yAngle * _radius + startPosition.position.y,
                0
            );

            // Rotate the projectile using Atan2 to get the angle between the projectile and the center of the circle
            float angleInDegrees = (float)(Mathf.Atan2(yAngle, xAngle) - Math.PI/2) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);

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

            // Bomb shrapnel is considered a bomb type when it hits a player
            boxCollider.AddComponent<Projectile>();

            // Bomb shrapnel movement isn't computed in the Projectile script
            rigidbody.linearVelocity = _projectileSpeed * projectile.transform.up;
            
            // Move the projectile shrapnel in the direction of the angle
            // rigidbody.linearVelocity = new Vector2(
            //     xAngle * _projectileSpeed * Time.deltaTime,
            //     yAngle * _projectileSpeed * Time.deltaTime
            // );
        }
    }

    void DrawDebugCircle(Vector2 center, float radius, int segments, Color color) {
        float angleIncrement = Mathf.PI * 2 / segments;

        for (int i = 0; i < segments; i++) {
            float angle = i * angleIncrement;
            Vector2 pointA = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Vector2 pointB = center + new Vector2(Mathf.Cos(angle + angleIncrement) * radius, Mathf.Sin(angle + angleIncrement) * radius);

            Debug.DrawLine(pointA, pointB, color, 5.0f); 
        }
    }
}
