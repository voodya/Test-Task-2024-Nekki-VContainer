using Cysharp.Threading.Tasks;
using System;
using VContainer;

public interface IGameLoopService : IBootableAsync, IDisposable
{
    void StartGameLoop();
}


public class GameLoopService : IGameLoopService
{
    private IEnemySpawnService _enemySpawnService;
    private IMapGeneratorService _mapGeneratorService;
    private IRuntimeCharacterService _runtimeCharacterService;

    [Inject]
    public GameLoopService(
        IEnemySpawnService enemySpawnService,
        IMapGeneratorService mapGeneratorService,
        IRuntimeCharacterService runtimeCharacterService)
    {
        _enemySpawnService = enemySpawnService;
        _mapGeneratorService = mapGeneratorService;
        _runtimeCharacterService = runtimeCharacterService;
        Priority = 1000;
    }


    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public UniTask Boot()
    {
        return UniTask.CompletedTask;   
    }

    public void StartGameLoop()
    {
        _runtimeCharacterService.StartPlayingFromPoint(_mapGeneratorService.Ground.StartPose);
        _enemySpawnService.StartEnemySpawn();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }


}
