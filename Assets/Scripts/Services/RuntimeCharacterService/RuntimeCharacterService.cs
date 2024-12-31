using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer;

public interface IRuntimeCharacterService : IBootableAsync, IDisposable
{
    Transform CharacterTransform { get; }
    Rigidbody CharacterRb { get; }
    float Speed { get; }

    void StartPlayingFromPoint(Vector3 point);
}


public class RuntimeCharacterService : IRuntimeCharacterService
{
    private Func<CharacterSaveData, CharacterPresenter> _factory;
    private ISaveService _saveService;
    private CharacterSaveData _saveData;
    private CharacterPresenter _currentCharacter;
    private CompositeDisposable _disposable;
    private ISpellHolderService _spellHolderService;
    private IPlayerInputService _playerInputService;

    public Transform CharacterTransform => _currentCharacter.CharacterTransform;
    public Rigidbody CharacterRb => _currentCharacter.Rigidbody;
    public float Speed => _currentCharacter.Speed;

    [Inject]
    public RuntimeCharacterService(
        Func<CharacterSaveData, CharacterPresenter> func,
        ISaveService saveService,
        ISpellHolderService spellHolderService,
        IPlayerInputService playerInputService)
    {
        _factory = func;
        _saveService = saveService;
        Priority = 10;
        _spellHolderService = spellHolderService;
        _playerInputService = playerInputService;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    private Subject<Unit> _onDie = new();

    public IObservable<Unit> OnCharacterDied => _onDie;

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _disposable = new CompositeDisposable();
        if (_saveService.TryGetSave(out CharacterSaveData data))
        {
            _saveData = data;
        }
        else
        {
            _saveData = new CharacterSaveData();
        }
        _currentCharacter = _factory.Invoke(_saveData);
        _currentCharacter.OnDied.Subscribe(OnDied).AddTo(_disposable);
    }

    public void StartPlayingFromPoint(Vector3 point)
    {
        _currentCharacter.Spawn(point);
        _spellHolderService.SetIgnoreCollision(_currentCharacter.Collider);
        _playerInputService.OnInputAttack.Subscribe(_ => 
        {
            _spellHolderService.ThrowSpell(_currentCharacter.AttackPose, _currentCharacter.Rigidbody.transform.forward);
        
        }).AddTo(_disposable);
    }

    private void OnDied(Unit unit)
    {
        _onDie.OnNext(unit);
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}
