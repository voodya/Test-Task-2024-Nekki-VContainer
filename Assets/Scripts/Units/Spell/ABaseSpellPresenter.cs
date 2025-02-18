using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

public interface ISpellPresenter : IDisposable
{
    string SpellName { get; }
    float CoolDown { get; }
    Collider Collider { get; }

    void Throw(CharacterPresenter character);
}

public abstract class ABaseSpellPresenter : ISpellPresenter
{
    protected ISpellView _view;
    protected SpellModel _model;
    protected LayerMask _mask;
    protected ISpellHolderService _service;
    protected CompositeDisposable _disposable;

    protected ABaseSpellPresenter(List<ISpellView> views, SpellModel model, IObjectResolver objectResolver, LayerMask mask)
    {
        _view = views[0];
        _model = model;
        _view.Hide();
        _mask = mask;
        _service = objectResolver.Resolve<ISpellHolderService>();
    }

    public string SpellName => _model.Name;

    public float CoolDown => _model.CoolDown;

    public Collider Collider => _view.Collider;

    public virtual void Dispose()
    {
        MonoBehaviour.Destroy(_view.GameObject);
        _disposable?.Dispose();
    }

    public abstract void Throw(CharacterPresenter character);
}
