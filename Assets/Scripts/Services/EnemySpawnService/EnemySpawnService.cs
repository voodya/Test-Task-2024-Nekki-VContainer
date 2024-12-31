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
    private readonly Func<EnemyConfig, EnemyPresenter> _factory;
    private Queue<EnemyPresenter> _presenters;
    private IMapGeneratorService _mapGeneratorService;
    
    private EnemyConfig _config;
    private int _maxEnemyCount = 10;

    private CompositeDisposable _enemyPoolDisposable;

    [Inject]
    public EnemySpawnService(
        Func<EnemyConfig, EnemyPresenter> func,
        EnemyConfig enemyConfig,
        int enemyCount,
        IMapGeneratorService mapGeneratorService)
    {
        _config = enemyConfig;
        _factory = func;
        Priority = 1;
        _maxEnemyCount = enemyCount;
        _mapGeneratorService = mapGeneratorService;
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
            var pres = _factory.Invoke(_config);
                _enemyPoolDisposable.Add(pres);
            _presenters.Enqueue(pres);
            await UniTask.Yield();
        }
    }

    public void Dispose()
    {
        Debug.LogError("Dispose EnemySpawnService");
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
        return _mapGeneratorService.GetRandomPositin();
    }
}
