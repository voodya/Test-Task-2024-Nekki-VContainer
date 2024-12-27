using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

[CreateAssetMenu(fileName = "SciptableScensRegister", menuName = "Installers/SciptableScensRegister")]

public class SciptableScensRegister : ScriptableInstaller
{
    [SerializeField] private AssetReference _loadViewScene;


    public override void Install(IContainerBuilder builder)
    {
        ScenesManagerService scenesManagerService = new ScenesManagerService();
        //Register scenes
        scenesManagerService.RegisterScene<LoadingView>(_loadViewScene);

        builder.RegisterInstance(scenesManagerService).As<ISceneManager>();
    }
}
