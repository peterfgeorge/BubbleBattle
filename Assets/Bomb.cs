using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private int _projectiles = 5;
    [SerializeField]
    private Sprite _projectileSprite;
    [SerializeField]
    private Sprite _bodySprite;

    void Start()
    {
        float diameter = _bodySprite.bounds.size.x;
        float arcStride = 360.0f / _projectiles;

        for (int i = 0; i < _projectiles; i++)
        {
            GameObject projectile = new GameObject("Projectile");
            projectile.transform.SetParent(transform);
            projectile.transform.localPosition = Vector3.zero;
            projectile.transform.localRotation = Quaternion.Euler(0, 0, i * arcStride);

            SpriteRenderer spriteRenderer = projectile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _projectileSprite;
            spriteRenderer.sortingOrder = 1;

            BoxCollider2D boxCollider = projectile.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(0.1f, 0.1f);
            boxCollider.isTrigger = true;

            Rigidbody2D rigidbody = projectile.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;
            rigidbody.linearVelocity = projectile.transform.up * 5;
        }
    }

    void Update()
    {
        
    }
}
