using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public interface IPlayerInputService : IBootableAsync, IDisposable
{
    IObservable<Unit> OnInputPrevious { get; }
    IObservable<Unit> OnInputNext { get; }
    IObservable<Unit> OnInputAttack { get; }
    IObservable<Vector2> OnInputMove { get; }
}


public class PlayerInputService : IPlayerInputService
{

    private Subject<Vector2> _onMove = new Subject<Vector2>();
    private Subject<Unit> _onAttack = new Subject<Unit>();
    private Subject<Unit> _onNext = new Subject<Unit>();
    private Subject<Unit> _onPrevious = new Subject<Unit>();
    private CompositeDisposable _disposable;
    private Vector2 _moveActionData;
    private InputAction _moveAction;

    [Inject]
    public PlayerInputService()
    {
        Priority = 100;
    }

    public IObservable<Vector2> OnInputMove => _onMove;
    public IObservable<Unit> OnInputAttack => _onAttack;
    public IObservable<Unit> OnInputNext => _onNext;
    public IObservable<Unit> OnInputPrevious => _onPrevious;

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _disposable = new CompositeDisposable(); 

        _moveAction = InputSystem.actions.FindAction("Move");

        if(_moveAction != null)
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => OnMove(_moveAction))
                .AddTo(_disposable);
            _moveAction.performed += SetMove;
            _moveAction.canceled += SetStay;
        }

        



        InputSystem.actions.FindAction("Attack").performed += OnAttack;
        InputSystem.actions.FindAction("Next").performed += OnNext;
        InputSystem.actions.FindAction("Previous").performed += OnPrevious;

        await UniTask.Yield();
    }

    private void SetStay(InputAction.CallbackContext context)
    {
        _moveActionData = Vector2.zero;
    }

    private void SetMove(InputAction.CallbackContext context)
    {
        _moveActionData = context.ReadValue<Vector2>();
    }

    public void Dispose()
    {
        _moveAction.performed -= SetMove;
        _moveAction.canceled -= SetStay;
        InputSystem.actions.FindAction("Attack").performed -= OnAttack;
        InputSystem.actions.FindAction("Next").performed -= OnNext;
        InputSystem.actions.FindAction("Previous").performed -= OnPrevious;
        _disposable?.Dispose();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _onAttack.OnNext(Unit.Default);
    }

    private void OnMove(InputAction context)
    {
        if(context.IsPressed())
        _onMove.OnNext(_moveActionData);
    }

    private void OnNext(InputAction.CallbackContext context)
    {
        _onNext.OnNext(Unit.Default);
    }

    private void OnPrevious(InputAction.CallbackContext context)
    {
        _onPrevious.OnNext(Unit.Default);
    }
}
