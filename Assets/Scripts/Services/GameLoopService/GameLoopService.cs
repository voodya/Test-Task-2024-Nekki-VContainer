using Cysharp.Threading.Tasks;
using System;
using UniRx;
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
    private IMovementService _rbMovementService;
    private ISpellHolderService _spellHolderService;
    private IGameUIService _gameUIService;
    private CompositeDisposable _compositeDisposable;
    private IApplicationScopesService _applicationScopesService;

    [Inject]
    public GameLoopService(
        IEnemySpawnService enemySpawnService,
        IMapGeneratorService mapGeneratorService,
        IRuntimeCharacterService runtimeCharacterService,
        IMovementService rbMovementService,
        ISpellHolderService spellHolderService,
        IGameUIService gameUIService,
        IApplicationScopesService applicationScopesService)
    {
        _enemySpawnService = enemySpawnService;
        _mapGeneratorService = mapGeneratorService;
        _runtimeCharacterService = runtimeCharacterService;
        Priority = 1000;
        _rbMovementService = rbMovementService;
        _spellHolderService = spellHolderService;
        _gameUIService = gameUIService;
        _applicationScopesService = applicationScopesService;
    }


    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public UniTask Boot()
    {
        _compositeDisposable = new CompositeDisposable();   
        return UniTask.CompletedTask;   
    }

    public void StartGameLoop()
    {
        _runtimeCharacterService.StartPlayingFromPoint(_mapGeneratorService.Ground.StartPose);
        _enemySpawnService.StartEnemySpawn();
        _rbMovementService.StartInputHandle();
        _spellHolderService.StartSpellHolder();
        _gameUIService.StartGameUI();
        _runtimeCharacterService.OnCharacterDied.Subscribe(OnDied).AddTo(_compositeDisposable);
    }

    private void OnDied(Unit unit)
    {
        _applicationScopesService.TryChangeScope(LocalScope.Menu);
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }


}
