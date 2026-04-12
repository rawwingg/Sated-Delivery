using UnityEngine;
using UnityEngine.AI;

// ─────────────────────────────────────────────────────────────
//  RouteFollower
//  Moves an object from Start → End → Start, then stops.
//  Drop on the object you want to move.
//  Assign the RouteVisualizer from your scene in the Inspector.
// ─────────────────────────────────────────────────────────────
public class RouteFollower : MonoBehaviour
{
    [Header("Route")]
    public Mission Visualizer;

    [Header("Movement")]
    public float Speed = 5f;
    [Tooltip("How close to a waypoint before moving to the next one")]
    public float WaypointThreshold = 0.2f;
    public bool MoveOnStart = true;

    [Header("Rotation")]
    public bool FaceDirection = true;
    public float RotationSpeed = 8f;

    private Vector3[] _corners;
    private int _currentIndex;
    private bool _movingForward = true;
    private bool _isMoving = false;

    void Start()
    {
        if (MoveOnStart) StartMoving();
    }

    void Update()
    {
        if (!_isMoving || _corners == null || _corners.Length == 0) return;
        MoveAlongPath();
    }

    // ── Public API ────────────────────────────────────────────

    public void StartMoving()
    {
        if (Visualizer == null)
        {
            Debug.LogError("[RouteFollower] No RouteVisualizer assigned.");
            return;
        }

        _corners = Visualizer.GetPathCorners();

        if (_corners == null || _corners.Length < 2)
        {
            Debug.LogWarning("[RouteFollower] Path has fewer than 2 points. " +
                             "Make sure RouteVisualizer has called ShowRoute() first.");
            return;
        }

        transform.position = _corners[0];
        _currentIndex = 1;
        _movingForward = true;
        _isMoving = true;
    }

    public void StopMoving() { _isMoving = false; }
    public void ToggleMoving() { if (_isMoving) StopMoving(); else StartMoving(); }

    // ── Internal ──────────────────────────────────────────────

    void MoveAlongPath()
    {
        Vector3 target = _corners[_currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position, target, Speed * Time.deltaTime);

        if (FaceDirection)
        {
            Vector3 dir = target - transform.position;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            }
        }

        if (Vector3.Distance(transform.position, target) < WaypointThreshold)
        {
            if (_movingForward)
            {
                if (_currentIndex < _corners.Length - 1)
                {
                    _currentIndex++;
                }
                else
                {
                    // Reached the end — reverse back to start
                    _movingForward = false;
                    _currentIndex = _corners.Length - 2;
                }
            }
            else
            {
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                }
                else
                {
                    // Back at start — stop
                    _isMoving = false;
                    Debug.Log("[RouteFollower] Round trip complete.");
                }
            }
        }
    }
}