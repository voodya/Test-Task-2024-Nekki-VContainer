using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using VContainer;
using VContainer.Unity;

public class BootstrapEntryPoint : IAsyncStartable
{
    private ILoadingPanelService _loadingPanelService;
    private IEnumerable<IBootableAsync> _bootableAsync;
    [Inject]
    public BootstrapEntryPoint(ILoadingPanelService loadingPanel, IEnumerable<IBootableAsync> bootableAsyncs)
    {
        _bootableAsync = bootableAsyncs;
        _loadingPanelService = loadingPanel;
    }

    public async UniTask StartAsync(CancellationToken cancellation = default)
    {
        await _loadingPanelService.Boot();
        _loadingPanelService.Show();
        foreach (var item in _bootableAsync)
        {
            await item.Boot();
        }
        await UniTask.Delay(5000);
        _loadingPanelService.Hide();
    }
}
