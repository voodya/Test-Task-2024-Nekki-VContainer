using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : ABaseScene
{
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _configureBtn;
    [SerializeField] private Button _exitBtn;

    public IObservable<Unit> OnPlay => _playBtn.OnClickAsObservable();
    public IObservable<Unit> OnConfigure => _configureBtn.OnClickAsObservable();
    public IObservable<Unit> OnExit => _exitBtn.OnClickAsObservable();


    public override void Dispose()
    {
        Debug.Log("Dispose MainMenuScreen");
    }
}
