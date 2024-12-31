using Cysharp.Threading.Tasks;
using DG.Tweening;
using VContainer;

public interface ILoadingPanelService : IBootableAsync
{
    void Hide();
    void PrintMessage(string message);
    void Release();
    void Show();
}


public class LoadingPanelService : ILoadingPanelService
{
    private LoadingView _view;

    public int Priority { get; set; }
    public bool IsBooted { get; set; }

    private ISceneManager _sceneManager;

    [Inject]
    public LoadingPanelService(ISceneManager sceneManager)
    {
        Priority = 0;
        _sceneManager = sceneManager;
    }

    public void PrintMessage(string message)
    {
        _view.MessageText.text = message;
    }

    public async UniTask Boot()
    {
        if(IsBooted) return;
        _view = await _sceneManager.GetScene<LoadingView>();
        IsBooted = true;
    }

    public void Show()
    {
        _view.CanvasGroup.DOFade(1f, 0.5f);
        _view.CanvasGroup.interactable = true;
        _view.CanvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _view.CanvasGroup.DOFade(0f, 0.5f);
        _view.CanvasGroup.interactable = false;
        _view.CanvasGroup.blocksRaycasts = false;
    }

    public void Release()
    {
        _sceneManager.ReleaseScene<LoadingView>();
    }
}
