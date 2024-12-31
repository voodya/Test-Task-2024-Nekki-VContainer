using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]

public class EnemyConfig : ScriptableObject
{
    [SerializeField] public int Hp;
    [SerializeField] public int Damage;
    [SerializeField] public float Protection;
    [SerializeField] public float AttackDelay;
}
