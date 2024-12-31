using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public interface ISpellView
{
    GameObject GameObject { get; }
    IObservable<Collision> OnCollided { get; }
    Rigidbody Rigidbody { get; }
    Collider Collider { get; }

    void Hide();
    void Land();
    void Throw();
}


public class SpellView : MonoBehaviour, ISpellView
{
    [SerializeField] private ParticleSystem _particlesProcess;
    [SerializeField] private ParticleSystem _particlesEnd;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private string _spellName;

    public Collider Collider => _collider;
    public string SpellViewName => _spellName;
    public IObservable<Collision> OnCollided => _collider.OnCollisionEnterAsObservable();
    public Rigidbody Rigidbody => _rigidbody;

    public GameObject GameObject => gameObject;

    public void Hide()
    {
        _rigidbody.isKinematic = true;
        gameObject.SetActive(false);
    }

    public void Throw()
    {
        gameObject.SetActive(true);
        _particlesProcess.Play();
    }

    public async void Land()
    {
        _particlesProcess.Stop();
        _particlesEnd.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(_particlesEnd.main.duration));
        Hide();
    }
}
