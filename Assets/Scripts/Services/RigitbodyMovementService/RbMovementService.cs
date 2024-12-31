using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer;

public interface IRbMovementService : IBootableAsync, IDisposable
{
    void StartInputHandle();
}


public class RbMovementService : IRbMovementService
{
    private IRuntimeCharacterService _characterService;
    private IPlayerInputService _playerInputService;
    private CompositeDisposable _disposables;
    private Rigidbody _attachedRigidbody;
    private Vector3 _currentInputVelocity;

    [Inject]
    public RbMovementService(
        IRuntimeCharacterService runtimeCharacterService,
        IPlayerInputService playerInputService)
    {
        _characterService = runtimeCharacterService;
        _playerInputService = playerInputService;
        Priority = 10;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _disposables = new CompositeDisposable();   
    }

    public void StartInputHandle()
    {
        _attachedRigidbody = _characterService.CharacterRb;

        _playerInputService.OnInputMove.Subscribe(vector => 
        {
            //Debug.LogError(vector);
            _currentInputVelocity.x = vector.x;
            _currentInputVelocity.z = vector.y;
        }).AddTo(_disposables);

        Observable
            .EveryFixedUpdate()
            .Subscribe(_ =>
            {
                _attachedRigidbody.linearVelocity = _currentInputVelocity * _characterService.Speed;
                if(_currentInputVelocity != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(_currentInputVelocity, Vector3.up); //funny(instantly) rotate effect lol
                    _attachedRigidbody.rotation = toRotation;
                }
                
                _currentInputVelocity = Vector3.zero;
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }
}
