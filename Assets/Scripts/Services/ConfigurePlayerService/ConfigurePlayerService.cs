using UniRx;
using VContainer;

public interface IConfigurePlayerService
{
    void Open(ConfigurePlayerView view);
}


public class ConfigurePlayerService : IConfigurePlayerService
{
    private ConfigurePlayerView _view;
    private ISceneManager _sceneManager;

    [Inject]
    public ConfigurePlayerService(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager;
    }

    public void Open(ConfigurePlayerView view)
    {
        view.OnExit.Subscribe(OnExit);
        view.OnHitPoint.Subscribe(ChangeHP);
        view.OnProtection.Subscribe(ChangeProtection);
    }

    private void ChangeProtection(float obj)
    {
        //throw new NotImplementedException(); 
    }

    private void ChangeHP(float obj)
    {
        //throw new NotImplementedException();
    }

    private void OnExit(Unit unit)
    {
        _sceneManager.ReleaseScene<ConfigurePlayerView>();
    }
}
