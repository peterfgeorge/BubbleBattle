using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public GameObject prefab; // The prefab to spawn (should be the ItemPickup prefab)
        public float spawnChance; // The rarity of the item (0-1, where 1 is 100% chance)
    }

    public ItemData[] items; // Array of items to spawn with their rarity
    public Vector2 spawnAreaMin = new Vector2(-12, -12); // Bottom-left corner of the spawn area
    public Vector2 spawnAreaMax = new Vector2(12, 12);   // Top-right corner of the spawn area
    public float spawnInterval = 5f; // Time in seconds between spawns
    public LayerMask wallLayerMask; // Layer mask to identify walls
    public float spawnCheckRadius = 0.5f; // Radius to check for walls

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnItem();
            timer = spawnInterval; // Reset the timer
        }
    }

    void SpawnItem()
    {
        GameObject selectedItem = GetRandomItem();

        if (selectedItem != null)
        {
            // Initialize spawnPosition with a default value
            Vector2 spawnPosition = Vector2.zero;

            // Try to find a valid spawn position
            int maxAttempts = 10; // Limit the number of attempts to prevent infinite loops
            bool validPositionFound = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                // Generate a random position within the spawn area
                spawnPosition = new Vector2(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                );

                // Check if the position is valid
                if (!Physics2D.OverlapCircle(spawnPosition, spawnCheckRadius, wallLayerMask))
                {
                    validPositionFound = true;
                    break;
                }
            }

            // If a valid position was found, spawn the item
            if (validPositionFound)
            {
                Instantiate(selectedItem, spawnPosition, Quaternion.identity);
            }
        }
    }

    GameObject GetRandomItem()
    {
        float totalChance = 0f;

        // Calculate the total spawn chance
        foreach (var item in items)
        {
            totalChance += item.spawnChance;
        }

        // Generate a random value between 0 and the total spawn chance
        float randomValue = Random.Range(0, totalChance);

        // Determine which item to spawn
        float cumulativeChance = 0f;
        foreach (var item in items)
        {
            cumulativeChance += item.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                return item.prefab; // Return the prefab of the selected item
            }
        }

        return null; // Fallback in case no item is selected
    }
}


