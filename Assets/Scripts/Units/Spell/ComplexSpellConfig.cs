using UnityEngine;

[CreateAssetMenu(fileName = "ComplexSpellConfig", menuName = "Configs/ComplexSpellConfig")]
public class ComplexSpellConfig : ScriptableObject
{
    [SerializeField] public PiecesSpellConfig Piece;
    [SerializeField] public ESpellType SpellType;
    [SerializeField] public string SpellName;
    [SerializeField] public Texture2D Icon;
}
