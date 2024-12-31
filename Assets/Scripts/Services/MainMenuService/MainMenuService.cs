using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using UniRx;

public interface IMainMenuService : IBootableAsync
{
    void Configure();
}


public class MainMenuService : IMainMenuService
{
    private MainMenuView _view;
    private ISceneManager _sceneManager;
    private IConfigurePlayerService _configurePlayerService;
    private IApplicationScopesService _stateMachineService;

    [Inject]
    public MainMenuService(
        ISceneManager sceneManager,
        IConfigurePlayerService configurePlayerService,
        IApplicationScopesService stateMachineService) 
    {
        _stateMachineService = stateMachineService;
        _configurePlayerService = configurePlayerService;
        _sceneManager = sceneManager;
        Priority = 1;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _view = await _sceneManager.GetScene<MainMenuView>();
    }

    public void Configure()
    {
        _view.OnConfigure.Subscribe(async _ =>
        {
            _configurePlayerService.Open(await _sceneManager.GetScene<ConfigurePlayerView>());
            Debug.LogError("On Configure");
        }).AddTo(_view);

        _view.OnExit.Subscribe(_ =>
        {
            Debug.LogError("On Exit");
            Application.Quit();
        }).AddTo(_view);

        _view.OnPlay.Subscribe(_ =>
        {
            Debug.LogError("On Play");
            _stateMachineService.TryChangeScope(LocalScope.Game);
        }).AddTo(_view);
    }
}
