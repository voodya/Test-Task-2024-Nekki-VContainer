public class SpellModel
{
    private float _speed;
    private float _range;
    private float _castRange;
    private float _liveTime;
    private int _damage;
    private int _maxPieceCount;
    private string _name;
    private float _coolDown;

    public int Damage => _damage;
    public float Range => _range;
    public float Speed => _speed;
    public float LiveTime => _liveTime;
    public string Name => _name;
    public float CoolDown => _coolDown;
    public float CastRange => _castRange;
    public int MaxPieceCount => _maxPieceCount;

    public SpellModel(PiecesSpellConfig config, string name)
    {
        _speed = config.Speed;
        _range = config.Range;
        _liveTime = config.LiveTime;
        _damage = config.Damage;
        _name = name;
        _coolDown = config.CoolDown;
        _castRange = config.CastRange;
        _maxPieceCount = config.MaxPieceCount;
    }
}
