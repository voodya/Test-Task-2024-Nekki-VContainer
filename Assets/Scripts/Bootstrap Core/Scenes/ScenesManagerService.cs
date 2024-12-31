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
    UniTask<T> GetScene<T>() where T : ABaseScene;
    void RegisterScene<T>(AssetReference assetReference) where T : ABaseScene;
    UniTask ReleaseAllScenes();
    UniTask ReleaseScene<T>() where T : ABaseScene;
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
        else
        {
            Debug.LogError($"{type} already registred");
        }
    }

    public async UniTask ReleaseAllScenes(List<Type> notReleasedScenes)
    {
        foreach (var item in _loadedScenes)
        {
            if (notReleasedScenes.Contains(item.Key))
                continue;
            await Addressables.UnloadSceneAsync(_loadedScenes[item.Key].Item2);
        }

        ValidateScenes();
    }

    public async UniTask ReleaseAllScenes()
    {
        foreach (var item in _loadedScenes)
        {
            if (item.Key == typeof(LoadingView))
                continue;
            //_loadedScenes[item.Key].Item1.Hide(true);
            await Addressables.UnloadSceneAsync(_loadedScenes[item.Key].Item2);
        }
        ValidateScenes();
    }

    private void ValidateScenes()
    {
        List<Type> types = new List<Type>();
        foreach (var item in _loadedScenes)
        {
            if (item.Value.Item1 == null)
                types.Add(item.Key);
        }
        foreach (var type in types)
        {
            _loadedScenes.Remove(type);
        }
    }


    public async UniTask<T> GetScene<T>()
        where T : ABaseScene
    {

        Type screenType = typeof(T);
        if (_loadedScenes.ContainsKey(screenType))
        {
            return _loadedScenes[screenType].Item1 as T;
        }
        else
        {
            AsyncOperationHandle<SceneInstance> loadingOperation = Addressables.LoadSceneAsync(_registredScenes[screenType], LoadSceneMode.Additive);

            T scene = (await loadingOperation).Scene.GetRoot<T>();
            _loadedScenes[screenType] = (scene, loadingOperation);
            await scene.Show();
            return scene;
        }
    }

    public async UniTask ReleaseScene<T>()
        where T : ABaseScene
    {
        Type screenType = typeof(T);

        if (_loadedScenes.ContainsKey(screenType))
        {
            await _loadedScenes[screenType].Item1.Hide();
            _loadedScenes[screenType].Item1.Dispose();
            await Addressables.UnloadSceneAsync(_loadedScenes[screenType].Item2);
            _loadedScenes.Remove(screenType);
        }
        else
            Debug.LogError($"No instance scene {screenType}");

        ValidateScenes();

    }
}
