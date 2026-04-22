using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class Mission : MonoBehaviour
{
    public int id;
    public MissionData missionData;

    [Header("Route Points")]
    public Transform StartPoint;
    public Transform EndPoint;

    [Header("Visuals")]
    public Color RouteColor = new Color(0.2f, 0.8f, 1f, 1f);
    public float LineWidth = 0.3f;
    public float HeightOffset = 0.1f;

    [Header("Behaviour")]
    public bool ShowOnStart = false;

    private LineRenderer _line;
    private NavMeshPath _path;
    private Vector3[] _corners;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _path = new NavMeshPath();

        _line.startWidth = LineWidth;
        _line.endWidth = LineWidth;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = RouteColor;
        _line.endColor = RouteColor;
        _line.positionCount = 0;
        _line.enabled = false;
    }

    private void Start()
    {
        if (ShowOnStart)
            ShowRoute();
    }

    public void ShowRoute()
    {
        if (!ValidatePoints())
            return;

        NavMesh.CalculatePath(StartPoint.position, EndPoint.position, NavMesh.AllAreas, _path);
        if (_path.status == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogWarning("[Mission] No valid NavMesh path found.");
            return;
        }

        _corners = _path.corners;
        _line.positionCount = _corners.Length;
        for (int i = 0; i < _corners.Length; i++)
            _line.SetPosition(i, _corners[i] + Vector3.up * HeightOffset);

        _line.enabled = true;
    }

    public void HideRoute() => _line.enabled = false;

    public void ToggleRoute()
    {
        if (_line.enabled) HideRoute();
        else ShowRoute();
    }

    public void ClearRoute()
    {
        _line.positionCount = 0;
        _line.enabled = false;
        _path.ClearCorners();
        _corners = null;
    }

    public void SetPoints(Transform start, Transform end)
    {
        StartPoint = start;
        EndPoint = end;
        ShowRoute();
    }

    public Vector3[] GetPathCorners() => _corners;

    private bool ValidatePoints()
    {
        if (StartPoint == null || EndPoint == null)
        {
            Debug.LogError("[Mission] StartPoint or EndPoint is not assigned.");
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
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
