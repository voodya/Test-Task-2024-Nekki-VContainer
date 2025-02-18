using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

public interface IEnemySpawnService : IBootableAsync, IDisposable
{
    void ReleaseEnemy(EnemyPresenter presenter);
    void StartEnemySpawn();
}


public class EnemySpawnService : IEnemySpawnService
{
    private readonly Func<EnemyPresenter> _factory;
    private Queue<EnemyPresenter> _presenters;
    private IMapGeneratorService _mapGeneratorService;
    private ICameraService _cameraService;
    
    private EnemyConfig _config;
    private int _maxEnemyCount = 10;

    private CompositeDisposable _enemyPoolDisposable;

    [Inject]
    public EnemySpawnService(
        Func<EnemyPresenter> func,
        int enemyCount,
        IMapGeneratorService mapGeneratorService,
        ICameraService cameraService)
    {
        _factory = func;
        Priority = 1;
        _maxEnemyCount = enemyCount;
        _mapGeneratorService = mapGeneratorService;
        _cameraService = cameraService;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _enemyPoolDisposable = new CompositeDisposable();
        _presenters = new Queue<EnemyPresenter>();
        for (int i = 0; i < _maxEnemyCount; i++)
        {
            var pres = _factory.Invoke();
                _enemyPoolDisposable.Add(pres);
            _presenters.Enqueue(pres);
            await UniTask.Yield();
        }
    }

    public void Dispose()
    {
        _enemyPoolDisposable?.Dispose();
    }

    public void StartEnemySpawn()
    {
        Observable
            .Timer(TimeSpan.FromSeconds(3f))
            .Repeat()
            .Subscribe(SpawnEnemy)
            .AddTo(_enemyPoolDisposable);
    }

    public void ReleaseEnemy(EnemyPresenter presenter)
    {
        _presenters.Enqueue(presenter);
    }

    private void SpawnEnemy(long obj)
    {
        if (_presenters.TryDequeue(out EnemyPresenter presenter))
        {
            presenter.Spawn(GetOutRectPosition());
        }
        else
        {
            Debug.LogError("Queue empty");
        }
    }


    private Vector3 GetOutRectPosition()
    {
        Vector3 pose = _mapGeneratorService.GetRandomPositin();
        Vector3[] poses = new Vector3[] { pose };
        var palnes = GeometryUtility.CalculateFrustumPlanes(_cameraService.CurrentCamera);
        var bounds = GeometryUtility.CalculateBounds(poses, Matrix4x4.identity);
        if (GeometryUtility.TestPlanesAABB(palnes, bounds))
        {
            //Debug.LogError("We in bounds");
            
            return GetOutRectPosition();
        }
        else
        {
            //Debug.LogError("We out bounds");
            return pose;
        }
        // too large, need optimize 
    }
}
