using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(CharacterData), menuName = "ScriptableObject/" + nameof(CharacterData))]
    public class CharacterData : ScriptableObject
    {
        //PlayerStatusとEnemyStatusの共通部分
        [SerializeField, Label("最大HP")] public int maxHp;
        [SerializeField, Label("攻撃力")] public int atk;
        [SerializeField, Label("防御力")] public int def;
        [SerializeField, Label("移動速度")] public int moveSpeed;
    }
}