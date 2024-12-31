using UniRx;

public class CharacterModel
{
    private IReactiveProperty<float> _hp;
    private int _damage;
    private float _protection;

    public IReactiveProperty<float> HP => _hp;
    public int Damage => _damage;
    public float Protection => _protection;

    public CharacterModel(CharacterSaveData data)
    {
        _hp = new ReactiveProperty<float>(data.HP);
        _damage = data.Damage;
        _protection = data.Protection;
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

    public CharacterSaveData()
    {
        HP = 100;
        Damage = 1;
        Protection = 0.5f;
    }
}

