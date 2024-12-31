using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public interface IMapGeneratorService : IBootableAsync
{
    public GroundView Ground { get; }

    Vector3 GetRandomPositin();
}


public class MapGeneratorService : IMapGeneratorService
{
    private List<GameObject> _obstacleVariants;
    private GroundView _groundPfb;
    private GroundView _groundInstance;
    private Vector4 _bounds;

    public GroundView Ground => _groundInstance;

    [Inject]
    public MapGeneratorService(List<GameObject> obstacles, GroundView groundView)
    {
        _obstacleVariants = obstacles;
        _groundPfb = groundView;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        _groundInstance = MonoBehaviour.Instantiate(_groundPfb);
        _bounds = _groundInstance.GroundBounds;
        await GenerateMap();
    }

    private async UniTask GenerateMap()
    {
        int obstacleCount = Random.Range(10, 15);
        Vector3 randomPose = _groundInstance.StartPose;
        for (int i = 0; i < obstacleCount; i++)
        {
            Transform tr = MonoBehaviour.Instantiate(_obstacleVariants[Random.Range(0, _obstacleVariants.Count)], _groundInstance.GroundTransform).transform;
            tr.position = GetRandomPositin();
        }

        _groundInstance.NavMeshSurface.BuildNavMesh();
        await UniTask.Yield();
    }

    public Vector3 GetRandomPositin()
    {
        return new Vector3(UnityEngine.Random.Range(_bounds.x, _bounds.z), 0f, UnityEngine.Random.Range(_bounds.y, _bounds.w));
    }
}



