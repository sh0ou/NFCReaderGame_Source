using UnityEngine;
using static Jubatus.TokenStorage;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(WeaponData), menuName = "ScriptableObject/" + nameof(WeaponData))]
    public class WeaponData : ScriptableObject
    {
        public GameObject model;
        public Vector3 position, rotation;
        public int atk;
        public TokenCat color;
        public int cost;
        public WeaponType type;
        public HandType handType;
    }

    public enum WeaponType : int
    {
        Item_Heal = -1, //回復アイテム
        None = 0,       //なし(素手)
        Sword = 1,      //剣
        AKGun = 2,      //銃(AK)
        HandGun = 3,    //銃(ハンドガン)
    }

    public enum HandType { None, Left, Right };
}