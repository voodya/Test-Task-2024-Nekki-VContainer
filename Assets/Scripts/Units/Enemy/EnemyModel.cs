using UniRx;

public class EnemyModel
{
    private IReactiveProperty<float> _hp;
    private float _protection;
    private int _damage;
    private float _attackDelay;
    private float _speed;

    public IReactiveProperty<float> Hp => _hp;
    public int Damage => _damage;
    public float Protection => _protection;
    public float AttackDelay => _attackDelay;

    public float Speed => _speed;

    public EnemyModel(EnemyConfig config)
    {
        _hp = new ReactiveProperty<float>(config.Hp);
        _damage = config.Damage;
        _protection = config.Protection;
        _attackDelay = config.AttackDelay;
        _speed = config.Speed;  
    }


    public void GetDamage(float damage)
    {
        _hp.Value -= damage * _protection;
    }
}
