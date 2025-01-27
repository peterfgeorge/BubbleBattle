using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offset;
    private Vector3 velocity;
    public float smoothTime = 0.5f;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();

        // Add player to camera targets
        GameObject activePlayers = GameObject.Find("ActivePlayers");
        Transform[] players = new Transform[activePlayers.transform.childCount];
        for (int i = 0; i < activePlayers.transform.childCount; i++)
        {
            players[i] = activePlayers.transform.GetChild(i);
        }
        foreach (Transform player in players)
        {
            if (player == activePlayers.transform) // Skip the parent GameObject
                continue;

            targets.Add(player);
        }
    }

    void LateUpdate () {
        if (targets.Count == 0) {
            return;
        }

        Move();
        Zoom();
    }

    void Zoom() {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    void Move() {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    float GetGreatestDistance() {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint() {
        if (targets.Count == 1) {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
