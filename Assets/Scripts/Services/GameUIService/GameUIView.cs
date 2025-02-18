using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameUIView : ABaseScene
{
    [SerializeField] private RawImage _spellImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _spellCooldown;
    [SerializeField] private Button _returnBtn;

    public IObservable<Unit> OnReturn => _returnBtn.OnClickAsObservable();

    public void SetSpell(ComplexSpellConfig spellConfig)
    {
        _spellImage.texture = spellConfig.Icon;
    }

    public void SetCooldown(string cooldownString)
    {
        _spellCooldown.text = cooldownString;
    }

    public void SetHP(string hpText)
    {
        _hpText.text = hpText;
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
