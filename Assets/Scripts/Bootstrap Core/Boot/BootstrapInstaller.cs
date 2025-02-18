using VContainer;
using VContainer.Unity;

public class BootstrapInstaller : SceneInstaller
{
    protected override void Configure(IContainerBuilder builder)
    {

        base.Configure(builder);
        builder.RegisterEntryPoint<BootstrapEntryPoint>();
    }
}
