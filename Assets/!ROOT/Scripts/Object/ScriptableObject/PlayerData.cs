using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(PlayerData), menuName = "ScriptableObject/" + nameof(PlayerData))]
    public class PlayerData : ScriptableObject
    {
        public GameObject model;
        public Avatar avatar;
    }
}