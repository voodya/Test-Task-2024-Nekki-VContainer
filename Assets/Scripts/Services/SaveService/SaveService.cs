using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;
using Newtonsoft.Json;
using System;

public interface ISaveService : IBootableAsync, IDisposable
{
    void SetSave<T>(T data) where T : class;
    bool TryGetSave<T>(out T data) where T : class;
}


public class SaveService : ISaveService
{
    private Dictionary<string, string> _runtimeData;
    private const string SAVE_FILE_NAME = "DebugSaves.json";
    private string _savePath;


    [Inject]
    public SaveService()
    {
        Priority = 0;
        _savePath = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
    }

    public bool IsBooted { get; set; }
    public int Priority { get; set; }

    public async UniTask Boot()
    {
        if (IsBooted) return;
        IsBooted = true;
        await LoadSaves();
    }

    private async UniTask LoadSaves()
    {
        if(File.Exists(_savePath))
        {
            string rawSaveData = await File.ReadAllTextAsync(_savePath);
            _runtimeData = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawSaveData);
        }
        else
        {
            _runtimeData = new Dictionary<string, string>();
        }
    }

    public void SaveLocal()
    {
        File.WriteAllText(_savePath, JsonConvert.SerializeObject(_runtimeData));
    }

    public bool TryGetSave<T>(out T data) where T : class
    {
        data = default;
        string key = typeof(T).Name;
        if(_runtimeData.ContainsKey(key))
        {
            data = JsonConvert.DeserializeObject<T>(_runtimeData[key]);
            return true;
        }
        else { return false; }
    }

    public void SetSave<T>(T data) where T : class
    {
        string key = typeof(T).Name;
        _runtimeData[key] = JsonConvert.SerializeObject(data);
    }

    public void Dispose()
    {
        SaveLocal();
    }
}
