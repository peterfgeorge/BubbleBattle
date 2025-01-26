using UnityEngine;

public class Mover : MonoBehaviour
{
    void FixedUpdate()
    {
        // Move the object in the forward direction
        transform.Translate(Vector3.forward * 10 * Time.deltaTime, Space.World);
    }
}
