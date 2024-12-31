using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ABaseEntryPoint : IAsyncStartable
{
    protected IEnumerable<IBootableAsync> _bootablesAsync;
    protected ILoadingPanelService _loadingPanelService;

    [Inject]
    public ABaseEntryPoint(
        ILoadingPanelService loadingPanelService,
        IEnumerable<IBootableAsync> bootableAsyncs)
    {
        _bootablesAsync = bootableAsyncs;
        _loadingPanelService = loadingPanelService;
    }

    public virtual async UniTask StartAsync(CancellationToken cancellation = default)
    {
        _loadingPanelService.Show();
        foreach (var item in _bootablesAsync)
        {
            try
            {  
              _loadingPanelService.PrintMessage($"Load {item.GetType()}");
              await item.Boot();
            }
            catch (Exception e)
            {
                Debug.LogError($"Somthing wrong in {item.GetType()} {e}");
            }
        }
    }
}
