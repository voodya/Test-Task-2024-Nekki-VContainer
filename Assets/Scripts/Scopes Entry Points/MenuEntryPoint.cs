using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using VContainer;

public class MenuEntryPoint : ABaseEntryPoint
{
    private IMainMenuService _mainMenuService;
    
    [Inject]
    public MenuEntryPoint(
        ILoadingPanelService loadingPanelService,
        IEnumerable<IBootableAsync> bootableAsyncs,
        IMainMenuService mainMenuService)
        : base(loadingPanelService, bootableAsyncs)
    {
        _mainMenuService = mainMenuService;
    }

    public override async UniTask StartAsync(CancellationToken cancellation = default)
    {
        await base.StartAsync(cancellation);
        _mainMenuService.Configure();
        _loadingPanelService.Hide();
    }
}
