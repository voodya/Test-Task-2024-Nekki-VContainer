using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


public interface ICharacterView : IUnit
{
    IObservable<Collider> OnCollided { get; }
    Rigidbody Rb { get; }
    Vector3 AttackPose { get; }
    Collider Collider { get; }
}


public class CharacterView : MonoBehaviour, ICharacterView
{
    [SerializeField] private Collider _trigger;
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _attackTransform;

    public Collider Collider => _collider;

    public Rigidbody Rb => _rb;

    public Subject<float> _onDamaget = new Subject<float>();

    public IObservable<Collider> OnCollided => _trigger.OnTriggerEnterAsObservable();

    public IObservable<float> OnDamaget => _onDamaget;

    public Vector3 AttackPose => _attackTransform.position;

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
