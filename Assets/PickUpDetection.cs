using UnityEngine;

public class PickUpDetection : MonoBehaviour
{

    public string ItemName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"{collision.name} picked up {ItemName}");
            Destroy(gameObject);
        }
    }
}
