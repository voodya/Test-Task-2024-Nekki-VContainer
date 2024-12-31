using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public interface IUnit
{
    IObservable<float> OnDamaget { get; }

    void Hide();
    void SetAttack();
    void SetDamaged();
    void SetDie();
    void SetWalk();
    void Show();
}


public interface IEnemyView : IUnit
{
    NavMeshAgent Agent { get; }
    Subject<float> SetDamage { get; }
}

public class EnemyView : MonoBehaviour, IEnemyView
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    public NavMeshAgent Agent => _agent;

    private Subject<float> _onDamaget = new Subject<float>();

    public Subject<float> SetDamage => _onDamaget;
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
