using System;
using UniRx;
using UnityEngine;
using VContainer;

public class CharacterPresenter
{
    private ICharacterView _characterView;
    private CharacterModel _characterModel;
    private CompositeDisposable _disposables;

    private Subject<Unit> _onDied = new();

    public Transform CharacterTransform => _characterView.Rb.transform;
    public IObservable<Unit> OnDied => _onDied;

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
        _characterView.OnDamaget.Subscribe(GetDamage).AddTo(_disposables);
        _characterModel.HP.Subscribe(OnDamaged).AddTo(_disposables);
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
            _disposables?.Dispose();
        }
    }
}
