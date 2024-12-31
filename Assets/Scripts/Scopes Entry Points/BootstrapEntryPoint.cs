using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

public class BootstrapEntryPoint : ABaseEntryPoint
{
    private IApplicationScopesService _stateMachineService;
    private IObjectResolver _objectResolver;

    [Inject]
    public BootstrapEntryPoint(
        ILoadingPanelService loadingPanelService,
        IEnumerable<IBootableAsync> bootableAsyncs,
        IApplicationScopesService stateMachineService,
        IObjectResolver objectResolver)
        : base(loadingPanelService, bootableAsyncs)
    {
        _stateMachineService = stateMachineService;
        _objectResolver = objectResolver;
    }

    public override async UniTask StartAsync(CancellationToken cancellation = default)
    {
        await _loadingPanelService.Boot();

        _stateMachineService.RegisterScopeEntryPoint(new MenuScope(_objectResolver), LocalScope.Menu);
        _stateMachineService.RegisterScopeEntryPoint(new GameScope(_objectResolver), LocalScope.Game);

        await base.StartAsync(cancellation);

        if (!_stateMachineService.TryChangeScope(LocalScope.Menu))
        {
            Debug.LogError($"Somthing wrong in MenuState");
        }
    }
}
