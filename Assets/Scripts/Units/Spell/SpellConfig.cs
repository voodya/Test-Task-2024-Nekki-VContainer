using UnityEngine;

[CreateAssetMenu(fileName = "SpellConfig", menuName = "Configs/SpellConfig")]

public class SpellConfig : ScriptableObject
{
    [SerializeField] public string SpellName;
    [SerializeField] public int Damage;
    [SerializeField] public float Speed;
    [SerializeField] public float Range;
    [SerializeField] public float LiveTime;
    [SerializeField] public float CoolDown;
}
