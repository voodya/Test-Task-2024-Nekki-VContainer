using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public interface ISceneManager
{
    UniTask<T> LoadScene<T>() where T : ABaseScene;
    void RegisterScene<T>(AssetReference assetReference) where T : ABaseScene;
}


public class ScenesManagerService : ISceneManager
{

    private readonly Dictionary<Type, AssetReference> _registredScenes;
    private readonly Dictionary<Type, (ABaseScene, AsyncOperationHandle<SceneInstance>)> _loadedScenes;

    public ScenesManagerService()
    {
        _registredScenes = new Dictionary<Type, AssetReference>();
        _loadedScenes = new Dictionary<Type, (ABaseScene, AsyncOperationHandle<SceneInstance>)>();
    }

    public void RegisterScene<T>(AssetReference assetReference) where T : ABaseScene
    {
        Type type = typeof(T);
        if (!_registredScenes.ContainsKey(type))
            _registredScenes.Add(type, assetReference);
    }


    public async UniTask<T> LoadScene<T>()
        where T : ABaseScene
    {
        Type screenType = typeof(T);

        AsyncOperationHandle<SceneInstance> loadingOperation = Addressables.LoadSceneAsync(_registredScenes[screenType], LoadSceneMode.Additive);

        T scene = (await loadingOperation).Scene.GetRoot<T>();
        _loadedScenes[screenType] = (scene, loadingOperation);
        return scene;
    }
}
