using System;
using Unity.AI.Navigation;
using UnityEngine;

public class GroundView : MonoBehaviour, IDisposable
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private Transform _rectStart;
    [SerializeField] private Transform _rectEnd;

    public Vector4 GroundBounds => GetBounds();

    public NavMeshSurface NavMeshSurface => _navMeshSurface;
    public Vector3 StartPose => _playerSpawnPoint.localPosition;
    public Transform GroundTransform => _navMeshSurface.transform;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GetCenterPoint(_rectStart.position, _rectEnd.position), GetSize(_rectStart.position, _rectEnd.position));
    }



    private Vector3 GetCenterPoint(Vector3 start, Vector3 end)
    {
        return new Vector3((start.x + end.x) / 2, 1f, (start.z + end.z) / 2);
    }
    private Vector4 GetBounds()
    {
        return new Vector4(_rectStart.position.x, _rectStart.position.z, _rectEnd.position.x, _rectEnd.position.z);
    }

    private Vector3 GetSize(Vector3 start, Vector3 end)
    {
        return new Vector3(GetNormalizedValue(start.x, end.x), 2f, GetNormalizedValue(start.z, end.z));
    }

    private float GetNormalizedValue(float a, float b)
    {
        float x;
        if (Mathf.Sign(a) == Mathf.Sign(b))
            if (a > 0)
                x = b - a;
            else
                x = Mathf.Abs(a - b);
        else
            x = Mathf.Abs(a) + Mathf.Abs(b);
        return x;
    }

    public void Dispose()
    {
        Destroy(this.gameObject);
    }
}
