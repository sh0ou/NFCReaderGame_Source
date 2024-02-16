using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(EnemyData), menuName = "ScriptableObject/" + nameof(EnemyData))]
    public class EnemyData : ScriptableObject
    {
        public enum MoveType
        {
            None,   //動かない
            Chase,  //追尾
            Special //特定の動き
        }

        public enum AttackType
        {
            None,   //攻撃しない
            Melee,  //近距離
            Range   //遠距離
        }

        public GameObject model;
        public string tag = "Enemy";
        public Vector3[] colCenter, colSize;
        public MoveType moveType;
        public AttackType attackType;
        public int score;
    }
}