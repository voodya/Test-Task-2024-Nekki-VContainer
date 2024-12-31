using UniRx;

public class CharacterModel
{
    private IReactiveProperty<float> _hp;
    private int _damage;
    private float _protection;
    private float _speed;

    public IReactiveProperty<float> HP => _hp;
    public int Damage => _damage;
    public float Protection => _protection;

    public float Speed => _speed;

    public CharacterModel(CharacterSaveData data)
    {
        _hp = new ReactiveProperty<float>(data.HP);
        _damage = data.Damage;
        _protection = data.Protection;
        _speed = data.Speed;
    }

    public void GetDamage(float damage)
    {
        _hp.Value -= damage * _protection;
    }

}

public class CharacterSaveData
{
    public float HP { get; set; }
    public int Damage { get; set; }
    public float Protection { get; set; }

    public float Speed { get; set; }

    public CharacterSaveData()
    {
        HP = 100;
        Damage = 1;
        Protection = 0.5f;
        Speed = 5f;
    }
}

