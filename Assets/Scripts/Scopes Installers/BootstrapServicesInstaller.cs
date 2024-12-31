using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

[CreateAssetMenu(fileName = "BootstrapServicesInstaller", menuName = "Installers/GlobalScope/BootstrapServicesInstaller")]

public class BootstrapServicesInstaller : ScriptableInstaller
{
    [SerializeField] private AssetReference _loadViewScene;
    [SerializeField] private AssetReference _menuScene;
    [SerializeField] private AssetReference _configurePlayerScene;
    [SerializeField] private AssetReference _gameUiScene;
    [SerializeField] private List<ScopeInstallersList> _rawInstallers;
    public override void Install(IContainerBuilder builder)
    {
        builder.Register<LoadingPanelService>(Lifetime.Singleton)
            .As<ILoadingPanelService>()
            .As<IBootableAsync>();

        builder.Register<SaveService>(Lifetime.Singleton)
            .As<ISaveService>()
            .As<IBootableAsync>();

        RegisterScenes(builder);
        RegisterStateMachine(builder);
        RegisterScopes(builder);
    }

    private void RegisterScopes(IContainerBuilder builder)
    {
        Dictionary<LocalScope, List<ScriptableInstaller>> installers = new();

        foreach (var item in _rawInstallers)
        {
            installers[item.Scope] = item.Installers;
        }

        builder.Register<ScopesHolderService>(Lifetime.Singleton)
            .As<IScopesHolderService>()
            .WithParameter("installers", installers);
    }

    private void RegisterScenes(IContainerBuilder builder)
    {
        ScenesManagerService scenesManagerService = new ScenesManagerService();
        //Register scenes
        scenesManagerService.RegisterScene<LoadingView>(_loadViewScene);
        scenesManagerService.RegisterScene<MainMenuView>(_menuScene);
        scenesManagerService.RegisterScene<ConfigurePlayerView>(_configurePlayerScene);
        scenesManagerService.RegisterScene<GameUIView>(_gameUiScene);

        builder.RegisterInstance(scenesManagerService).As<ISceneManager>();
    }

    private void RegisterStateMachine(IContainerBuilder builder)
    {
        builder.Register<ApplicationScopesService>(Lifetime.Singleton)
            .As<IApplicationScopesService>();
    }
}

[System.Serializable]
public class ScopeInstallersList
{
    public LocalScope Scope;
    public List<ScriptableInstaller> Installers;

}
