using System;
using UniRx;
using UnityEngine;
using VContainer;

public class EnemyPresenter
{
    private IEnemyView _view;
    private EnemyModel _model;
    private CompositeDisposable _disposable;
    private IEnemySpawnService _spawnService;
    private IRuntimeCharacterService _runtimeCharacterService;

    public EnemyPresenter(IEnemyView view, EnemyModel model, IObjectResolver objectResolver)
    {
        _view = view;
        _model = model;
        _spawnService = objectResolver.Resolve<IEnemySpawnService>();
        _runtimeCharacterService = objectResolver.Resolve<IRuntimeCharacterService>();
        _view.Hide();
    }


    public void Spawn(Vector3 position)
    {
        _view.Agent.Warp(position);
        _view.Show();
        UpdateTarget();
        _disposable = new CompositeDisposable();
        _view.OnDamaget.Subscribe(GetDamage).AddTo(_disposable);
        _model.Hp.Subscribe(OnDamaged).AddTo(_disposable);
        Observable
            .Timer(TimeSpan.FromSeconds(0.5f))
            .Repeat()
            .Subscribe(_ => UpdateTarget())
            .AddTo(_disposable);
    }

    private void UpdateTarget()
    {
        _view.Agent.SetDestination(_runtimeCharacterService.CharacterTransform.position);
    }

    private void GetDamage(float obj)
    {
        _model.GetDamage(obj);
    }

    private void OnDamaged(float hp)
    {
        _view.SetDamaged();
        if (hp <= 0)
        {
            _view.SetDie();
            _spawnService.ReleaseEnemy(this);
            _disposable?.Dispose();
        }
    }

}
