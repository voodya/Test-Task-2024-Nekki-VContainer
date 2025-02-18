using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using VContainer;

public interface IMovementService : IBootableAsync, IDisposable
{
    void StartInputHandle();
}

public class RbMovementService : IMovementService
{
    private IRuntimeCharacterService _characterService;
    private IPlayerInputService _playerInputService;
    private CompositeDisposable _disposables;
    private Rigidbody _attachedRigidbody;
    private Vector3 _currentInputVelocity;
    private ICameraService _cameraService;
    private LayerMask _layerMask;
    private GizmosHelper.DrawFunc _drawFunc;
    private Vector3 _hit;
    private Vector2 _mousePosition;
    private IGizmosHelper _gizmoDrawer;

    [Inject]
    public RbMovementService(
        IRuntimeCharacterService runtimeCharacterService,
        IPlayerInputService playerInputService,
        ICameraService cameraService,
        LayerMask layerMask,
        IGizmosHelper gizmosHelper)
    {
        _characterService = runtimeCharacterService;
        _playerInputService = playerInputService;
        Priority = 10;
        _cameraService = cameraService;
        _layerMask = layerMask;
        _gizmoDrawer = gizmosHelper;
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
            _currentInputVelocity.x = vector.x;
            _currentInputVelocity.z = vector.y;
        })
            .AddTo(_disposables);

        _playerInputService.OnMouseMove.Subscribe(vector =>
        {
            _mousePosition = vector;
        })
            .AddTo(_disposables);

        _drawFunc = () => 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_hit, 1f);
        };

        _gizmoDrawer.AddGizmos(_drawFunc);

        Observable
            .EveryFixedUpdate()
            .Subscribe(_ =>
            {
                _attachedRigidbody.linearVelocity = _currentInputVelocity * _characterService.Speed;
                if (Physics
                    .Raycast(
                    _cameraService.CurrentCamera.ScreenPointToRay(_mousePosition),
                    out RaycastHit hit,
                    1000,
                    _layerMask))
                {
                    _hit = hit.point;
                    Debug.LogError("Looked TEST");
                    _attachedRigidbody.transform.LookAt(hit.point, _attachedRigidbody.transform.up);
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

public class NavMeshMoveService : IMovementService
{
    private IRuntimeCharacterService _characterService;
    private IPlayerInputService _playerInputService;
    private ICameraService _cameraService;
    private CompositeDisposable _disposables;
    private NavMeshAgent _navMeshAgent;
    private LayerMask _layerMask;
    private IGizmosHelper _gizmoDrawer;
    private Vector3 target;
    private Vector3 orogin;
    private Vector3 hit;
    private GizmosHelper.DrawFunc _drawFunc;

    [Inject]
    public NavMeshMoveService(
        IRuntimeCharacterService runtimeCharacterService,
        IPlayerInputService playerInputService,
        ICameraService cameraService,
        LayerMask layerMask,
        IGizmosHelper gizmoDrawer)
    {
        _characterService = runtimeCharacterService;
        _playerInputService = playerInputService;
        Priority = 10;
        _cameraService = cameraService;
        _layerMask = layerMask;
        _gizmoDrawer = gizmoDrawer;
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
        _drawFunc = () =>
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(orogin, target - orogin);
        };
        _gizmoDrawer.AddGizmos(_drawFunc);
        _navMeshAgent = _characterService.CharacterRb.AddComponent<NavMeshAgent>();
        _navMeshAgent.acceleration = 1000;
        _navMeshAgent.speed = 7;
        _navMeshAgent.angularSpeed = 5000;
        _navMeshAgent.radius = 0.3f;
        _navMeshAgent.Warp(_characterService.CharacterRb.position);
        _playerInputService.OnMouseMove.Subscribe(_ => HandleMouseDirectly()).AddTo(_disposables);
    }

    private void HandleMouseDirectly()
    {
        Debug.LogError("TEST Mouse handled");
        target = _cameraService.CurrentCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue(), Camera.MonoOrStereoscopicEye.Mono);
        Ray ray = _cameraService.CurrentCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        orogin = _cameraService.CurrentCamera.transform.position;
        Vector3 direction = target - orogin;
        if(Physics.Raycast(ray, out RaycastHit hit, 1000, _layerMask))
        {
            this.hit = hit.point; 
            _navMeshAgent.SetDestination(hit.point);
        }
    }

    public void Dispose()
    {
        _gizmoDrawer.RemoveGizmos(_drawFunc);
        _disposables?.Dispose();
    }
}
