using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using VContainer;

public class GameEntryPoint : ABaseEntryPoint
{
    private IGameLoopService _gameLoopService;

    [Inject]
    public GameEntryPoint(
        ILoadingPanelService loadingPanelService,
        IEnumerable<IBootableAsync> bootableAsyncs,
        IGameLoopService gameLoopService) 
        : base(loadingPanelService, bootableAsyncs)
    {
        _gameLoopService = gameLoopService;
    }

    public override async UniTask StartAsync(CancellationToken cancellation = default)
    {
        await base.StartAsync(cancellation);
        _loadingPanelService.Hide();
        await UniTask.Delay(3000);
        _gameLoopService.StartGameLoop();
    }

}
