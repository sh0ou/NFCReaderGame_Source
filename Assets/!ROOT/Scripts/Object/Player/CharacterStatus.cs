using UnityEngine;

namespace Jubatus
{
    public class CharacterStatus : MonoBehaviour
    {
        [SerializeField] public CharacterData characterData;
        [SerializeField] public WeaponData weaponData;
        [SerializeField, Label("現在HP")] public int hp;
        [SerializeField, Label("最大HP")] public int maxHp;
        [SerializeField, Label("攻撃力")] public int atk;
        [SerializeField, Label("防御力")] public int def;

        [SerializeField, Label("無敵時間")] public float invincibleTime = 0f;

        private void Awake()
        {
            if (characterData)
            {
                maxHp = characterData.maxHp;
                hp = maxHp;
                atk = characterData.atk + weaponData.atk;
                def = characterData.def;
            }
            else
            {
                Debug.LogWarning($"CharacterData未設定:{transform.root.name}", this);
            }
        }
    }
}