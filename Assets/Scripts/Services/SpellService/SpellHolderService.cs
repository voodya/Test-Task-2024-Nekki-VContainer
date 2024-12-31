using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

public interface ISpellHolderService: IBootableAsync, IDisposable
{
    IObservable<Dictionary<string, TimeSpan>> OnCooldownsUpdated { get; }
    IObservable<string> OnCurrentSpellChanget { get; }

    void ReleaseSpell(SpellPresenter spellPresenter);
    void SetCoolDown(SpellPresenter spellPresenter);
    void SetIgnoreCollision(Collider collider);
    void StartSpellHolder();
    void ThrowSpell(Vector3 startPose, Vector3 direction);
}


public class SpellHolderService : ISpellHolderService
{
    private Dictionary<string, Queue<SpellPresenter>> _currentSpellsPool;
    private Dictionary<string, TimeSpan> _spellsCoolDowns;
    private Dictionary<string, DateTime> _lastThrow;
    private Dictionary<string, float> _coolDowns;
    private readonly Func<SpellConfig, SpellPresenter> _factory;
    private string _currentSpell;
    private int _currentSpellId;
    private int _maxSpellCount;
    private CompositeDisposable _compositeDisposable;
    private List<SpellConfig> _spellConfigs;
    private IPlayerInputService _playerInputService;

    private Subject<string> _onSpellChanget = new Subject<string>();

    private Subject<Dictionary<string, TimeSpan>> _onCoolDownsUpdated = new Subject<Dictionary<string, TimeSpan>>();

    public IObservable<Dictionary<string, TimeSpan>> OnCooldownsUpdated => _onCoolDownsUpdated;

    public IObservable<string> OnCurrentSpellChanget => _onSpellChanget;

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    [Inject]
    public SpellHolderService(
        List<SpellConfig> spells,
        int spellPoolSize,
        Func<SpellConfig, SpellPresenter> factory,
        IPlayerInputService playerInputService)
    {
        _maxSpellCount = spellPoolSize;
        _spellConfigs = spells;
        _factory = factory;
        Priority = 1;
        _playerInputService = playerInputService;
    }

    public void SetIgnoreCollision(Collider collider)
    {
        foreach (var item in _currentSpellsPool)
        {
            foreach(var presenter in item.Value)
            {
                Physics.IgnoreCollision(collider, presenter.Collider);
            }
        }
    }

    public void SetCoolDown(SpellPresenter spellPresenter)
    {
        _lastThrow[spellPresenter.SpellName] = DateTime.Now;
    }

    public void ThrowSpell(Vector3 startPose, Vector3 direction)
    {
        if (_spellsCoolDowns[_currentSpell].TotalMilliseconds > 0) return;
        _currentSpellsPool[_currentSpell].Dequeue().Throw(startPose, direction);
    }


    public void ReleaseSpell(SpellPresenter spellPresenter)
    {
        _currentSpellsPool[spellPresenter.SpellName].Enqueue(spellPresenter);
    }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _compositeDisposable = new CompositeDisposable();
        _lastThrow = new();
        _currentSpellsPool = new();
        _spellsCoolDowns = new();
        _coolDowns = new();
        _currentSpell = _spellConfigs[0].SpellName;
        _currentSpellId = 0;
        foreach (var item in _spellConfigs)
        {
            Queue<SpellPresenter> spellsQueue = new Queue<SpellPresenter>();
            for (var i = 0; i < _maxSpellCount; i++)
            {
                spellsQueue.Enqueue(_factory.Invoke(item));
                
            }
            _spellsCoolDowns[item.SpellName] = new TimeSpan();
            _currentSpellsPool[item.SpellName] = spellsQueue;
            _lastThrow[item.SpellName] = DateTime.Now;
            _coolDowns[item.SpellName] = item.CoolDown;
            await UniTask.Yield();
        }
    }

    public void StartSpellHolder()
    {
        Observable
            .EveryFixedUpdate()
            .Subscribe(UpdateCooldowns)
            .AddTo(_compositeDisposable);
        _playerInputService.OnInputNext.Subscribe(_ => 
        {
            if (_currentSpellId == _spellConfigs.Count - 1)
                _currentSpellId = 0;
            else
                _currentSpellId++;
            _currentSpell = _spellConfigs[_currentSpellId].SpellName;
            _onSpellChanget.OnNext(_currentSpell);
        }).AddTo(_compositeDisposable);
        _playerInputService.OnInputPrevious.Subscribe(_ =>
        {
            if (_currentSpellId == 0)
                _currentSpellId = _spellConfigs.Count - 1;
            else
                _currentSpellId--;
            _currentSpell = _spellConfigs[_currentSpellId].SpellName;
            _onSpellChanget.OnNext(_currentSpell);
        }).AddTo(_compositeDisposable);
    }

    private void UpdateCooldowns(long obj)
    {
        foreach (var item in _spellConfigs)
        {
            _spellsCoolDowns[item.SpellName] = _lastThrow[item.SpellName] - DateTime.Now + TimeSpan.FromSeconds(_coolDowns[item.SpellName]);
        }
        _onCoolDownsUpdated.OnNext(_spellsCoolDowns);
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }
}
