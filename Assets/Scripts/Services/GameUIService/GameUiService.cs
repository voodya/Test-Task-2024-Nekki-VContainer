using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using VContainer;

public interface IGameUIService : IBootableAsync, IDisposable
{
    void StartGameUI();
}


public class GameUiService : IGameUIService
{
    private GameUIView _view;
    private CompositeDisposable _compositeDisposable;
    private ISceneManager _sceneManager;
    private ISpellHolderService _spellHolderService;
    private IRuntimeCharacterService _runtimeCharacterService;
    private Dictionary<string, ComplexSpellConfig> _configs;
    private IApplicationScopesService _applicationScopesService;

    [Inject]
    public GameUiService(
        ISceneManager sceneManager,
        ISpellHolderService spellHolderService,
        IRuntimeCharacterService runtimeCharacterService,
        Dictionary<string, ComplexSpellConfig> configs,
        IApplicationScopesService applicationScopesService)
    {
        _configs = configs;
        _sceneManager = sceneManager;
        _spellHolderService = spellHolderService;
        _runtimeCharacterService = runtimeCharacterService;
        Priority = 1;
        _applicationScopesService = applicationScopesService;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _compositeDisposable = new CompositeDisposable();
        _view = await _sceneManager.GetScene<GameUIView>();


    }

    private void SetCharacterHP(float obj)
    {
        if (obj > 0)
            _view.SetHP(obj.ToString());
        else
            _view.SetHP("Died");
    }

    public void StartGameUI()
    {
        _view.OnReturn.Subscribe(ReturnToMenu).AddTo(_compositeDisposable);
        _view.SetSpell(_configs[_spellHolderService.CurrentSpell]);
        _spellHolderService.OnCooldownsUpdated.Subscribe(UpdateCoolDownView).AddTo(_compositeDisposable);
        _spellHolderService.OnCurrentSpellChanget.Subscribe(ChangeSpell).AddTo(_compositeDisposable);
        _runtimeCharacterService.CharacterHP.Subscribe(SetCharacterHP).AddTo(_compositeDisposable);
    }

    private void ReturnToMenu(Unit unit)
    {
        _applicationScopesService.TryChangeScope(LocalScope.Menu);
    }

    private void ChangeSpell(string obj)
    {
        _view.SetSpell(_configs[obj]);
    }

    private void UpdateCoolDownView(Dictionary<string, TimeSpan> dictionary)
    {
        if (dictionary[_spellHolderService.CurrentSpell].TotalMilliseconds < 0)
            _view.SetCooldown("Ready");
        else
            _view.SetCooldown(dictionary[_spellHolderService.CurrentSpell].TotalSeconds.ToString());
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }
}





