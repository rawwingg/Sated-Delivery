using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

// ─────────────────────────────────────────────────────────────
//  RouteVisualizer
//  Drop this on any GameObject in your scene.
//  Assign StartPoint and EndPoint in the Inspector.
//  Call ShowRoute() / HideRoute() from UI, other scripts,
//  or via the public bool ShowOnStart.
// ─────────────────────────────────────────────────────────────
[RequireComponent(typeof(LineRenderer))]
public class Mission : MonoBehaviour
{
    public int id;
    public GameObject driver;

    [Header("Route Points")]
    public Transform StartPoint;
    public Transform EndPoint;

    [Header("Visuals")]
    public Color RouteColor = new Color(0.2f, 0.8f, 1f, 1f);
    public float LineWidth = 0.3f;
    [Tooltip("How high above the road surface the line floats")]
    public float HeightOffset = 0.1f;

    [Header("Behaviour")]
    public bool ShowOnStart = false;

    private LineRenderer _line;
    private NavMeshPath _path;
    private Vector3[] _corners;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _path = new NavMeshPath();

        // Configure the LineRenderer
        _line.startWidth = LineWidth;
        _line.endWidth = LineWidth;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = RouteColor;
        _line.endColor = RouteColor;
        _line.positionCount = 0;
        _line.enabled = false;
    }

    void Start()
    {
        if (ShowOnStart) ShowRoute();
    }

    // ── Public API ────────────────────────────────────────────

    /// <summary>Calculate the road path and draw it.</summary>
    public void ShowRoute()
    {
        if (!ValidatePoints()) return;

        NavMesh.CalculatePath(StartPoint.position, EndPoint.position,
                              NavMesh.AllAreas, _path);

        if (_path.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("[RouteVisualizer] No valid NavMesh path found. " +
                             "Make sure your roads are baked into the NavMesh.");
            return;
        }

        // Copy corners, raise each point slightly off the surface
        _corners = _path.corners;
        _line.positionCount = _corners.Length;
        for (int i = 0; i < _corners.Length; i++)
            _line.SetPosition(i, _corners[i] + Vector3.up * HeightOffset);

        _line.enabled = true;
        Debug.Log($"[RouteVisualizer] Route drawn with {_corners.Length} points. " +
                  $"Status: {_path.status}");
    }

    /// <summary>Hide the route line without clearing the path.</summary>
    public void HideRoute()
    {
        _line.enabled = false;
    }

    /// <summary>Toggle route visibility.</summary>
    public void ToggleRoute()
    {
        if (_line.enabled) HideRoute();
        else ShowRoute();
    }

    /// <summary>Fully clear the line and path data.</summary>
    public void ClearRoute()
    {
        _line.positionCount = 0;
        _line.enabled = false;
        _path.ClearCorners();
        _corners = null;
    }

    /// <summary>Update start/end at runtime then redraw.</summary>
    public void SetPoints(Transform start, Transform end)
    {
        StartPoint = start;
        EndPoint = end;
        ShowRoute();
    }

    /// <summary>Returns the path corners for RouteFollower to use.</summary>
    public Vector3[] GetPathCorners()
    {
        return _corners;
    }

    // ── Helpers ───────────────────────────────────────────────

    bool ValidatePoints()
    {
        if (StartPoint == null || EndPoint == null)
        {
            Debug.LogError("[RouteVisualizer] StartPoint or EndPoint is not assigned.");
            return false;
        }
        return true;
    }

    // Draw start/end gizmos in the Scene view
    void OnDrawGizmos()
    {
        if (StartPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(StartPoint.position, 0.4f);
        }
        if (EndPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(EndPoint.position, 0.4f);
        }
    }
}