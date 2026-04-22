using UnityEngine;

public class RouteFollower : MonoBehaviour
{
    [Header("Route")]
    public Mission Visualizer;

    [Header("Movement")]
    public float Speed = 5f;
    public float WaypointThreshold = 0.2f;
    public bool MoveOnStart = true;

    [Header("Rotation")]
    public bool FaceDirection = true;
    public float RotationSpeed = 8f;

    [Header("Height")]
    public float HeightOffset = 0.25f;

    private Vector3[] _corners;
    private int _currentIndex;
    private bool _movingForward = true;
    private bool _isMoving = false;

    void Start()
    {
        if (MoveOnStart)
            StartMoving();
    }

    void Update()
    {
        if (!_isMoving || _corners == null || _corners.Length == 0)
            return;

        MoveAlongPath();
    }

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
            Debug.LogWarning("[RouteFollower] Path has fewer than 2 points. Make sure Mission.ShowRoute() ran first.");
            return;
        }

        transform.position = _corners[0] + Vector3.up * HeightOffset;
        _currentIndex = 1;
        _movingForward = true;
        _isMoving = true;
    }

    public void StopMoving()
    {
        _isMoving = false;
    }

    public void ToggleMoving()
    {
        if (_isMoving) StopMoving();
        else StartMoving();
    }

    void MoveAlongPath()
    {
        Vector3 target = _corners[_currentIndex] + Vector3.up * HeightOffset;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            Speed * Time.deltaTime
        );

        if (FaceDirection)
        {
            Vector3 dir = target - transform.position;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    RotationSpeed * Time.deltaTime
                );
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
                    _isMoving = false;

                    if (Visualizer != null)
                    {
                        Visualizer.ClearRoute();
                        Destroy(Visualizer.gameObject);
                    }

                    Destroy(gameObject);

                    Debug.Log("[RouteFollower] Round trip complete.");
                }
            }
        }
    }
}