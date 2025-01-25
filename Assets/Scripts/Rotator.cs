using UnityEngine;

public class Rotator : MonoBehaviour
{
    private float _rotationSpeed = 200.0f;

    void Update()
    {
        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
    }
}
