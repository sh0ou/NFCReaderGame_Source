using FelicaLib;
using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(CardData), menuName = "ScriptableObject/" + nameof(CardData))]
    public class CardData : ScriptableObject
    {
        public string id;
        public SystemCode systemCode;
        public bool isAllowAllIDm; //システムコードが一致する場合、すべてのIDmを受け付ける
        public Sprite sprite;
        public WeaponData weaponData;
    }
}