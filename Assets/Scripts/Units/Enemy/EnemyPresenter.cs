using System;
using UniRx;
using UnityEngine;
using VContainer;

public class EnemyPresenter : IDisposable
{
    private IEnemyView _view;
    private EnemyModel _model;
    private CompositeDisposable _disposable;
    private IEnemySpawnService _spawnService;
    private IRuntimeCharacterService _runtimeCharacterService;
    private DateTime _attackTime;

    private bool _isActive;

    public EnemyPresenter(IEnemyView view, EnemyModel model, IObjectResolver objectResolver)
    {
        _attackTime = DateTime.Now;
        _view = view;
        _model = model;
        _spawnService = objectResolver.Resolve<IEnemySpawnService>();
        _runtimeCharacterService = objectResolver.Resolve<IRuntimeCharacterService>();
        _view.Hide();
        _disposable = new CompositeDisposable();
        _view.OnDamaget.Subscribe(GetDamage).AddTo(_disposable);
        _view.OnCollideCharacter.Subscribe(AttackCharacter).AddTo(_disposable);
        _model.Hp.Subscribe(OnHpChanget).AddTo(_disposable);
        Observable
            .Timer(TimeSpan.FromSeconds(0.5f))
            .Repeat()
            .Subscribe(_ => UpdateTarget())
            .AddTo(_disposable);
    }


    public void Spawn(Vector3 position)
    {
        _isActive = true;
        _view.Agent.Warp(position);
        _view.Show();
        _view.SetWalk();
        UpdateTarget();
    }

    private void AttackCharacter(ICharacterView view)
    {
        if (!_isActive) return;
        if ((DateTime.Now - _attackTime).TotalSeconds > _model.AttackDelay)
        {
            view.SetDamage.OnNext(_model.Damage);
            _view.SetAttack();
            _attackTime = DateTime.Now;
        }
    }

    private void UpdateTarget()
    {
        if (!_isActive) return;
        _view.Agent.SetDestination(_runtimeCharacterService.CharacterTransform.position);
    }

    private void GetDamage(float obj)
    {
        if (!_isActive) return;
        _model.GetDamage(obj);
    }

    private void OnHpChanget(float hp)
    {
        if (!_isActive) return;
        
        if (hp <= 0)
        {
            _view.SetDie();
            _isActive = false;
            _view.Agent.isStopped = true;
            _spawnService.ReleaseEnemy(this);
        }
        else
        {
            _view.SetDamaged();
        }
    }

    public void Dispose()
    {
        MonoBehaviour.Destroy(_view.GameObject);
        _disposable?.Dispose();
    }
}
