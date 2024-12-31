using System;
using UniRx;
using UnityEngine;
using VContainer;

public class CharacterPresenter : IDisposable
{
    private ICharacterView _characterView;
    private CharacterModel _characterModel;
    private CompositeDisposable _disposables;

    private Subject<Unit> _onDied = new();

    public Transform CharacterTransform => _characterView.Rb.transform;
    public float Speed => _characterModel.Speed;
    public Rigidbody Rigidbody => _characterView.Rb;
    public IObservable<Unit> OnDied => _onDied;

    public Vector3 AttackPose => _characterView.AttackPose;

    public Collider Collider => _characterView.Collider;

    public IReactiveProperty<float> CharacterHP => _characterModel.HP;

    public CharacterPresenter(ICharacterView view, CharacterModel model, IObjectResolver objectResolver)
    {
        _characterView = view;
        _characterModel = model;
        _characterView.Hide();
    }

    public void Spawn(Vector3 position)
    {
        _characterView.Rb.transform.position = position;
        _characterView.Show();
        _disposables = new CompositeDisposable();
        _characterView.OnCollided.Subscribe(TryGetDamage).AddTo(_disposables);
        _characterView.OnDamaget.Subscribe(GetDamage).AddTo(_disposables);
        _characterModel.HP.Subscribe(OnDamaged).AddTo(_disposables);
    }

    private void TryGetDamage(Collider collider)
    {
        if(collider.TryGetComponent(out IEnemyView enemyView))
        {
            enemyView.SetCharacter.OnNext(_characterView);
        }
    }

    private void GetDamage(float obj)
    {
        _characterModel.GetDamage(obj);
    }

    private void OnDamaged(float hp)
    {
        _characterView.SetDamaged();
        if (hp <= 0)
        {
            _characterView.SetDie();
            _onDied.OnNext(Unit.Default);
        }
    }

    public void Dispose()
    {
        MonoBehaviour.Destroy(_characterView.GameObject);
        _disposables?.Dispose();
    }
}
