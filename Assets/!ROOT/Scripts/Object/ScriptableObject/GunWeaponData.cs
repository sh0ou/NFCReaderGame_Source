using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = nameof(GunWeaponData), menuName = "ScriptableObject/" + nameof(GunWeaponData))]
    public class GunWeaponData : WeaponData
    {
        public GameObject bulletObj;
        public float bulletSpeed;
        public int bulletCount;
    }
}