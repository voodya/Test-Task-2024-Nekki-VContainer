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
    private ISaveService _saveService;

    private CharacterSaveData _characterData;

    [Inject]
    public ConfigurePlayerService(
        ISceneManager sceneManager,
        ISaveService saveService)
    {
        _sceneManager = sceneManager;
        _saveService = saveService;
    }

    public void Open(ConfigurePlayerView view)
    {
        if(!_saveService.TryGetSave(out _characterData))
        {
            _characterData = new CharacterSaveData();
        }
        _view = view;
        _view.SetCurrentValues(_characterData);
        view.OnExit.Subscribe(OnExit);
        view.OnHitPoint.Subscribe(ChangeHP);
        view.OnProtection.Subscribe(ChangeProtection);
    }

    private void ChangeProtection(float obj)
    {
        _characterData.Protection = obj;
    }

    private void ChangeHP(float obj)
    {
        _characterData.HP = obj;
    }

    private void OnExit(Unit unit)
    {
        _saveService.SetSave(_characterData);
        _sceneManager.ReleaseScene<ConfigurePlayerView>();
    }
}
