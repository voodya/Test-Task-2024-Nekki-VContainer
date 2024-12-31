using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


public interface ICharacterView : IUnit
{
    IObservable<Collider> OnCollided { get; }
    Rigidbody Rb { get; }
}


public class CharacterView : MonoBehaviour, ICharacterView
{
    [SerializeField] private Collider _trigger;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;

    public Rigidbody Rb => _rb;

    public Subject<float> _onDamaget = new Subject<float>();

    public IObservable<Collider> OnCollided => _trigger.OnTriggerEnterAsObservable();

    public IObservable<float> OnDamaget => _onDamaget;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetDie()
    {
        _animator.SetBool("Die", true);
    }

    public void SetWalk()
    {
        _animator.SetBool("Walk", true);
    }

    public void SetAttack()
    {
        _animator.SetTrigger("Attack");
    }

    public void SetDamaged()
    {
        _animator.SetTrigger("Damaget");
    }
}
