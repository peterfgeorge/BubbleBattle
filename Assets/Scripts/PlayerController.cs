using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed { get; private set; }
    public float rotationSpeed = 200f;
    private Vector3 targetScale; // The desired scale based on bubble count
    private float smoothSpeed = 5f; // The speed at which scaling occurs

    public float acceleration = 5f;  // The rate of acceleration
    public float deceleration = 5f;
    private Vector2 moveInput;  // Stores input from left stick
    private Vector2 rotateInput;  // Stores input from right stick
    private float currentSpeed = 0f;

    public InputActionReference move;
    public InputActionReference fire;
    public GameObject projectile;

    private bool isSlowed = false;

    private bool isSwordFishActive = false;

    private PlayerManager gameManager;
    private GameSceneController sceneManager;

    private PlayerInput playerInput;
    private int _playerIndex = -1;

    public int PlayerIndex
    {
        get { return _playerIndex; }
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _playerIndex = playerInput.playerIndex;
        Debug.Log($"Player {playerInput.playerIndex} using device: {playerInput.devices[0].displayName}");
    }

    private void Start() {
        moveSpeed = normalSpeed;
    }

    // Inventory system for one item
    private string currentItem = null;

    // Bubble count (score) and inflation settings
    public float inflationSpeed = 0.1f;  // How fast the player inflates
    public float inflationSensitivity = 10f;
    private float maxScale = 2f;  // Max inflation scale (adjust as needed)

    [SerializeField] private float bubbleCount = 0; // Starting bubbles
    [SerializeField] private float dashSpeed = 9f;    // Speed during dash
    [SerializeField] private float normalSpeed = 5f;  // Normal player speed
    [SerializeField] private float bubbleLossRate = .25f; // Bubbles lost per second while dashing
    private bool isDashing = false;  // Tracks if the player is dashing
    private Coroutine dashCoroutine;

    // Get bubble count with getter
    public float BubbleCount
    {
        get { return bubbleCount; }
    }

    private void Update()
    {
        moveInput = move.action.ReadValue<Vector2>();
        HandleInflation();  // Call to manage the inflation based on bubble count
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Read input from the left stick (Y-axis for forward/backward movement)
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        // Read input from the right stick (X-axis for rotation)
        rotateInput = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started) // When the dash button is pressed
        {
            StartDash();
        }
        else if (context.canceled) // When the dash button is released
        {
            StopDash();
        }
    }

    private void StartDash()
    {
        if (isDashing) return;

        isDashing = true;
        moveSpeed = dashSpeed;

        // Start losing bubbles over time
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private void StopDash()
    {
        if (!isDashing) return;

        isDashing = false;
        moveSpeed = normalSpeed;

        // Stop losing bubbles
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
        }
    }

    private IEnumerator DashCoroutine()
    {
        while (isDashing)
        {
            // Gradually reduce bubbles over time
            float bubblesToLose = bubbleLossRate * Time.deltaTime; // Scaled by frame time
            Debug.Log($"Bubbles to lose: {bubblesToLose}");
            bubbleCount = bubbleCount - bubblesToLose;

            Debug.Log($"Bubble count: {bubbleCount}");

            // If bubbles run out, stop the dash
            if (bubbleCount <= 0)
            {
                StopDash();
            }

            yield return null; // Wait for the next frame
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Determine the target speed based on the Y-axis of the move input
        float targetSpeed = moveInput.y * moveSpeed;

        // Gradually adjust current speed towards the target speed
        if (Mathf.Abs(targetSpeed) > 0.1f) // If there's input, accelerate
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else // If no input, decelerate to zero
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        // Apply movement in the character's forward direction (based on rotation)
        Vector2 forwardMovement = transform.up * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMovement);
    }

    private void HandleRotation()
    {
        // Rotate the character based on the X-axis of the rotate input
        float rotationAmount = -rotateInput.x * rotationSpeed * Time.fixedDeltaTime;

        // Apply rotation around the Z-axis (2D rotation)
        rb.MoveRotation(rb.rotation + rotationAmount);
    }


    public void ApplySlowEffect(float speedMultiplier, float duration)
    {
        if (isSlowed) return; // Avoid stacking slows

        isSlowed = true;
        float originalSpeed = moveSpeed;
        moveSpeed *= speedMultiplier; // Reduce speed by multiplier
        Debug.Log($"Player slowed! Speed reduced to {moveSpeed} for {duration} seconds.");

        // Restore the original speed after the duration
        StartCoroutine(RemoveSlowEffect(originalSpeed, duration));
    }

    private System.Collections.IEnumerator RemoveSlowEffect(float originalSpeed, float duration)
    {
        yield return new WaitForSeconds(duration); // Wait for the slow duration
        moveSpeed = originalSpeed; // Restore the original speed
        isSlowed = false; // Allow future slows
        Debug.Log("Player speed restored.");
    }

    private void OnEnable()
    {
        fire.action.started += Fire;
    }

    private void OnDisable()
    {
        fire.action.started -= Fire;
    }

    public void SetSwordFishActive(bool isActive)
    {
        isSwordFishActive = isActive;
    }

    // Modified Fire function to check for item in inventory and remove item after firing
    private void Fire(InputAction.CallbackContext obj)
    {
        if (currentItem != null)
        {
            Debug.Log($"Fired using item: {currentItem}");

            float offsetDistance = transform.localScale.x * 2f;  // Multiply scale by a factor (adjust as necessary)
            
            Vector3 spawnOffset = transform.up * offsetDistance;

            Vector3 spawnPosition = transform.position + spawnOffset;

            // Determine the type of projectile based on the current item
            Projectile.ProjectileType projectileType;

            Debug.Log("CURRENT ITEM: " + currentItem);
            if (currentItem == "Bomb")
            {
                projectileType = Projectile.ProjectileType.Bomb;
            }
            else if(currentItem == "SeaWeed")
            {
                projectileType = Projectile.ProjectileType.SeaWeed;
            }
            else if(currentItem == "SwordFish")
            {
                projectileType = Projectile.ProjectileType.SwordFish;
            }
            else
            {
                projectileType = Projectile.ProjectileType.Dart;
            }

            // Instantiate the projectile at the adjusted spawn position
            GameObject proj = Instantiate(projectile, spawnPosition, transform.rotation);

            proj.AddComponent<Projectile>();

            Destroy(proj.GetComponent<ItemFunctions>());

            // Set the direction for the projectile based on the player's rotation
            Vector2 fireDirection = transform.up; // Use the player's "up" direction as the fire direction
            proj.GetComponent<Projectile>().SetupProjectile(fireDirection, gameObject, projectileType);

            // Remove the item from the inventory
            currentItem = null;
            Debug.Log("Item used and removed from inventory.");

            Transform curProjectileTransform = transform.Find("CurProjectile");
            curProjectileTransform.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.Log("Cannot fire, no item in inventory.");
        }
    }



    private void HandleInflation()
    {
        // Calculate the target scale based on bubble count
        float inflationFactor = Mathf.Clamp01((float)bubbleCount / (float)inflationSensitivity); // Adjust 10f for sensitivity
        targetScale = Vector3.one + Vector3.one * inflationFactor * (maxScale - 1f);
    }

    // Function to pick up an item (Inventory check)
    public bool TryPickUpItem(string itemName)
    {
        if (isSwordFishActive)
        {
            Debug.Log("Cannot pick up items while SwordFish is active!");
            return false;
        }

        if (currentItem == null)
        {
            currentItem = itemName;
            Transform curProjectileTransform = transform.Find("CurProjectile");

            if (curProjectileTransform != null)
            {
                SpriteRenderer spriteRenderer = curProjectileTransform.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    GameObject prefab = Resources.Load<GameObject>($"Prefabs/{itemName}");

                    if (prefab != null)
                    {
                        SpriteRenderer prefabSpriteRenderer = prefab.GetComponent<SpriteRenderer>();

                        if (prefabSpriteRenderer != null)
                        {
                            spriteRenderer.sprite = prefabSpriteRenderer.sprite;
                            curProjectileTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
                        }
                        else
                        {
                            Debug.LogError($"Prefab {itemName} does not have a SpriteRenderer!");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Prefab {itemName} not found in Prefabs folder!");
                    }
                }
                else
                {
                    Debug.LogError("CurProjectile does not have a SpriteRenderer!");
                }
            }
            else
            {
                Debug.LogError("CurProjectile child GameObject not found!");
            }
            return true;
        }

        Debug.Log("Inventory full!");
        return false;
    }

    // Function to add bubbles (increases bubble count)
    public void AddBubbles(float count)
    {
        bubbleCount += count;
        Debug.Log($"Bubble count: {bubbleCount}");

        // Optionally, cap the bubble count
        if (bubbleCount > 100)  // Max bubble count limit (adjustable)
        {
            bubbleCount = 100;
        }
    }

    public void Death()
    {
        Reset();

        // Disable player components
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerController>().enabled = false;

        // Start the respawn coroutine
        StartCoroutine(RespawnPlayer());
    }

    public void Reset()
    {
        bubbleCount = 0;
        Debug.Log($"Bubble count: {bubbleCount}");

        currentItem = null;
        projectile = null;
        isSwordFishActive = false;
        Transform curProjectileTransform = transform.Find("CurProjectile");
        curProjectileTransform.GetComponent<SpriteRenderer>().sprite = null;
    }

    private IEnumerator RespawnPlayer()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<PlayerController>().enabled = false;

        // Disable all child GameObjects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Wait for the respawn delay (3 seconds)
        yield return new WaitForSeconds(3f);

        // Find the SpawnPoints GameObject and choose a random child spawn point
        GameObject spawnPoints = GameObject.Find("SpawnPoints");
        if (spawnPoints == null)
        {
            Debug.LogError("SpawnPoints GameObject not found!");
            yield break;
        }

        int randomIndex = Random.Range(0, spawnPoints.transform.childCount);
        Transform chosenSpawnPoint = spawnPoints.transform.GetChild(randomIndex);

        // Move the player to the spawn point
        transform.position = chosenSpawnPoint.position;

        // Re-enable the player GameObject and all its components
        gameObject.SetActive(true);

        // Re-enable player components
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<PlayerController>().enabled = true;

        // Re-enable all child GameObjects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void OnPause(InputAction.CallbackContext obj) {
        sceneManager = GameObject.Find("SceneManager").GetComponent<GameSceneController>();
        sceneManager.PauseGame();
    }
}
