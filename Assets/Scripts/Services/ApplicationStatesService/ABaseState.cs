using UnityEngine;
using VContainer;
using VContainer.Unity;

public interface IABaseState
{
    void Enter(LocalScope scope);
    void Exit();
}


public abstract class ABaseState<T> : IABaseState where T : ABaseEntryPoint
{
    protected ISceneManager _sceneManager;
    protected readonly LifetimeScope _currentScope;
    protected IScopesHolderService _scopesHolderService;

    protected LifetimeScope NewScope;

    [Inject]
    public ABaseState(IObjectResolver objectResolver)
    {
        _sceneManager = objectResolver.Resolve<ISceneManager>();
        _currentScope = objectResolver.Resolve<LifetimeScope>();
        _scopesHolderService = objectResolver.Resolve<IScopesHolderService>();
    }

    public virtual void Enter(LocalScope scope)
    {
        NewScope = _currentScope.CreateChild<LifetimeScope>(builder =>
        {
            var installers = _scopesHolderService.GetServices(scope);
            foreach (var installer in installers)
            {
                installer.Install(builder);
            }
            builder.RegisterEntryPoint<T>();
            builder.RegisterBuildCallback(c => Debug.LogError($"{scope} scope created"));
        });
    }
    public virtual void Exit()
    {
        NewScope.Dispose();
        _sceneManager.ReleaseAllScenes();
    }
}
