using UnityEngine;
using VContainer;

[CreateAssetMenu(fileName = "MenuServicesInstaller", menuName = "Installers/MenuScope/MenuServicesInstaller")]

public class MenuServicesInstaller : ScriptableInstaller
{
    public override void Install(IContainerBuilder builder)
    {
        builder.Register<MainMenuService>(Lifetime.Scoped)
            .As<IMainMenuService>()
            .As<IBootableAsync>();
        builder.Register<ConfigurePlayerService>(Lifetime.Scoped)
            .As<IConfigurePlayerService>();
    }
}
