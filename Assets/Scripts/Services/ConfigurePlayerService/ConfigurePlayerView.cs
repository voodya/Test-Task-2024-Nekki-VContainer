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

    [SerializeField] private TextMeshProUGUI _hpValueText;
    [SerializeField] private TextMeshProUGUI _protectopnValueText;

    public IObservable<float> OnHitPoint => _sliderHP.ObserveEveryValueChanged(x => x.value);
    public IObservable<float> OnProtection => _sliderProtection.ObserveEveryValueChanged(x => x.value);
    public IObservable<Unit> OnExit => _closeBtn.OnClickAsObservable();

    public async override UniTask Show()
    {
        await base.Show();
    }

    public void SetCurrentValues(CharacterSaveData data)
    {
        OnHitPoint.Subscribe(value => _hpValueText.text = value.ToString()).AddTo(this);
        OnProtection.Subscribe(value => _protectopnValueText.text = value.ToString()).AddTo(this);
        _sliderHP.value = data.HP;
        _sliderProtection.value = data.Protection;

    }

    public override void Dispose()
    {
        Debug.Log("Close configure screen");
    }
}
