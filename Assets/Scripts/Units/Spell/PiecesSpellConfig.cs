using UnityEngine;

[CreateAssetMenu(fileName = "SpellConfig", menuName = "Configs/SpellConfig")]

public class PiecesSpellConfig : ScriptableObject
{
    [SerializeField] public string SpellName;
    [SerializeField] public int Damage;
    [SerializeField] public int MaxPieceCount;
    [SerializeField] public float Speed;
    [SerializeField] public float Range;
    [SerializeField] public float LiveTime;
    [SerializeField] public float CoolDown;
    [SerializeField] public float CastRange;
    [SerializeField] public Texture2D Icon;
}
