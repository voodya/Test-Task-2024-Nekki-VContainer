using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ConfigurePlayerView : ABaseScene
{
    [SerializeField] private Slider _sliderHP;
    [SerializeField] private Slider _sliderProtection;
    [SerializeField] private Button _closeBtn;

    public IObservable<float> OnHitPoint => _sliderHP.ObserveEveryValueChanged(x => x.value);
    public IObservable<float> OnProtection => _sliderProtection.ObserveEveryValueChanged(x => x.value);
    public IObservable<Unit> OnExit => _closeBtn.OnClickAsObservable();

    public override UniTask Show()
    {
        return base.Show();
    }

    public override void Dispose()
    {
        Debug.Log("Close configure screen");
    }
}
