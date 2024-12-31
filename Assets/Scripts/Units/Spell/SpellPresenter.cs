using System;
using UniRx;
using UnityEngine;
using VContainer;

public class SpellPresenter
{
    private ISpellView _view;
    private SpellModel _model;
    private CompositeDisposable _disposable;
    private LayerMask _mask;
    private ISpellHolderService _service;

    public string SpellName => _model.Name;
    public float CoolDown => _model.CoolDown;

    public Collider Collider => _view.Collider;

    public SpellPresenter(ISpellView spellView, SpellModel spellModel, IObjectResolver objectResolver, LayerMask layerMask)
    {
        _mask = layerMask;
        _view = spellView;
        _view.Hide();
        _model = spellModel;
        _service = objectResolver.Resolve<ISpellHolderService>();
    }


    public void Throw(Vector3 pose, Vector3 direction)
    {
        _disposable = new CompositeDisposable();
        _view.Throw();
        _service.SetCoolDown(this);
        _view.Rigidbody.isKinematic = false;
        _view.Rigidbody.linearVelocity = Vector3.zero;
        _view.Rigidbody.transform.position = pose;
        _view.Rigidbody.AddForce(direction * _model.Speed);
        _view.OnCollided.Subscribe(OnSpellColided).AddTo(_disposable);
        Observable
            .Timer(TimeSpan.FromSeconds(_model.LiveTime))
            .Subscribe(_ => 
            {
                _view.Rigidbody.isKinematic = true;
                _view.Rigidbody.linearVelocity = Vector3.zero;
                _view.Land();
                _disposable?.Dispose();
                _service.ReleaseSpell(this);
            }).AddTo(_disposable);
    }

    private void OnSpellColided(Collision collision)
    {
        var colliders = Physics.OverlapSphere(collision.contacts[0].point, _model.Range, _mask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out IEnemyView enemyView))
            {
                enemyView.SetDamage.OnNext(_model.Damage);
            }
        }
        _view.Rigidbody.isKinematic = true;
        _view.Rigidbody.linearVelocity = Vector3.zero;
        _view.Land();
        _disposable?.Dispose();
        _service.ReleaseSpell(this);
    }
}
