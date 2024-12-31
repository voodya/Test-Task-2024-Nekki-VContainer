public class SpellModel
{
    private float _speed;
    private float _range;
    private float _liveTime;
    private int _damage;
    private string _name;
    private float _coolDown;

    public int Damage => _damage;
    public float Range => _range;
    public float Speed => _speed;
    public float LiveTime => _liveTime;
    public string Name => _name;
    public float CoolDown => _coolDown;

    public SpellModel(SpellConfig config)
    {
        _speed = config.Speed;
        _range = config.Range;
        _liveTime = config.LiveTime;
        _damage = config.Damage;
        _name = config.SpellName;
        _coolDown = config.CoolDown;
    }
}
