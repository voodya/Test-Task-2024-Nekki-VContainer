using UniRx;

public class EnemyModel
{
    private IReactiveProperty<float> _hp;
    private float _protection;
    private int _damage;

    public IReactiveProperty<float> Hp => _hp;
    public int Damage => _damage;
    public float Protection => _protection;

    public EnemyModel(EnemyConfig config)
    {
        _hp = new ReactiveProperty<float>(config.Hp);
        _damage = config.Damage;
        _protection = config.Protection;
    }


    public void GetDamage(float damage)
    {
        _hp.Value -= damage * _protection;
    }
}
