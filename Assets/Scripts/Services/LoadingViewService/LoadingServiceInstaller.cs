using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "LoadingServiceInstaller", menuName = "Installers/LoadingServiceInstaller")]
public class LoadingServiceInstaller : ScriptableInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        builder.Register<LoadingPanelService>(Lifetime.Singleton)
            .As<ILoadingPanelService>()
            .As<IBootableAsync>();
    }
}
