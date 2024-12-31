using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public interface ICameraService : IBootableAsync
{
    Camera CurrentCamera { get; }
}


public class CameraService : ICameraService
{
    private Camera _cameraInstance;
    private Camera _cameraPfb;

    public Camera CurrentCamera => _cameraInstance;

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    [Inject]
    public CameraService(Camera camera)
    {
        _cameraPfb = camera;
    }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        _cameraInstance = MonoBehaviour.Instantiate(_cameraPfb);
        await UniTask.Yield();
    }
}
