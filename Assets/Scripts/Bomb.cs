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

    private float _radius = 0;

    void Awake() 
    {
        float localScale = transform.localScale.x;
        Debug.Log("localScale: " + localScale);
        float diameter = GetComponent<SpriteRenderer>().bounds.size.x  * Mathf.Sign(transform.localScale.x);
        _radius =  diameter / 2;
        //Debug.Log("Awake: " + _radius);
    }

    void Start() 
    {
        Invoke(nameof(Detonate), _detonationTime);
    }

    void Update()
    {
        DrawDebugCircle(transform.position, _radius, _projectiles, Color.red);
    }

    private void Detonate() 
    {
        for (int i = 0; i < _projectiles; i++)
        {
            float theta = i * Mathf.PI * 2 / _projectiles;
            Vector2 direction = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Debug.Log("i. " + i + " - Direction: " + direction);
            SetupProjectile(direction);
        }
    }

    private void SetupProjectile(Vector2 direction) 
    {
        GameObject projectile = new GameObject("Projectile");
        projectile.transform.SetParent(transform);

        projectile.transform.localPosition = new Vector3(direction.x * _radius, direction.y * _radius, 0);

        // Rotate the projectile using Atan2 to get the angle between the projectile and the center of the circle
        float angleInDegrees = (float)(Mathf.Atan2(direction.y, direction.x) - Math.PI/2) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angleInDegrees);

        projectile.transform.localScale = new Vector3(_projectileScale, _projectileScale, 1);

        SpriteRenderer projectileSpriteRenderer = projectile.AddComponent<SpriteRenderer>();
        projectileSpriteRenderer.sprite = _projectileSprite;

        BoxCollider2D boxCollider = projectile.AddComponent<BoxCollider2D>();

        float projectileSize = projectileSpriteRenderer.bounds.size.x;
        boxCollider.size = new Vector2(projectileSize, projectileSize);
        boxCollider.isTrigger = true;

        Rigidbody2D rigidbody = projectile.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;

        // Bomb shrapnel is considered a bomb type when it hits a player
        boxCollider.AddComponent<Projectile>();

        // Bomb shrapnel movement isn't computed in the Projectile script
        rigidbody.linearVelocity = _projectileSpeed * direction;
    }

    private void DrawDebugCircle(Vector2 center, float radius, int segments, Color color) {
        float angleIncrement = Mathf.PI * 2 / segments;

        for (int i = 0; i < segments; i++) {
            float angle = i * angleIncrement;
            Vector2 pointA = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Vector2 pointB = center + new Vector2(Mathf.Cos(angle + angleIncrement) * radius, Mathf.Sin(angle + angleIncrement) * radius);

            Debug.DrawLine(pointA, pointB, color, 100.0f); 
        }
    }
}
