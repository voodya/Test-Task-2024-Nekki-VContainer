using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

public class ForwardSpellPresenter: ABaseSpellPresenter
{
    public ForwardSpellPresenter(List<ISpellView> views, SpellModel model, IObjectResolver objectResolver, LayerMask mask) : base(views, model, objectResolver, mask)
    {
    }

    public override void Throw(CharacterPresenter character)
    {
        Vector3 pose = character.AttackPose;
        Vector3 direction = character.CharacterTransform.forward;
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
