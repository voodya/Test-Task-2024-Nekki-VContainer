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

        InputSystem.actions.FindAction("Move").performed += OnMove;
        InputSystem.actions.FindAction("Attack").performed += OnAttack;
        InputSystem.actions.FindAction("Next").performed += OnNext;
        InputSystem.actions.FindAction("Previous").performed += OnPrevious;

        await UniTask.Yield();
    }

    public void Dispose()
    {
        InputSystem.actions.FindAction("Move").performed -= OnMove;
        InputSystem.actions.FindAction("Attack").performed -= OnAttack;
        InputSystem.actions.FindAction("Next").performed -= OnNext;
        InputSystem.actions.FindAction("Previous").performed -= OnPrevious;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _onAttack.OnNext(Unit.Default);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _onMove.OnNext(context.ReadValue<Vector2>());
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
