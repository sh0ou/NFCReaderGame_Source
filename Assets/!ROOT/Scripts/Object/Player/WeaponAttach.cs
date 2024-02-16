using UnityEngine;

namespace Jubatus
{
    public class WeaponAttach : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private WeaponData defaultWeapon;
        [SerializeField, UnEditable] private WeaponData currentWeapon;
        [SerializeField, UnEditable] private GameObject weaponModel = null;
        [SerializeField, UnEditable] private Collider weaponCol;
        [SerializeField, UnEditable] private Transform handPos_L, handPos_R;

        private void Awake()
        {
            GetHand(defaultWeapon);
            SetWeapon(defaultWeapon);
            SetLayer();
        }

        private void Update()
        {
            //手のボーンが取得されてない場合は取得
            //if (handPos_L == null || handPos_R == null) GetHand();
        }

        public void GetHand(WeaponData data)
        {
            if (data.handType == HandType.None) return;

            handPos_L = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            handPos_R = anim.GetBoneTransform(HumanBodyBones.RightHand);
        }

        public GameObject GetWeaponModel() => weaponModel;

        public void SetWeapon(WeaponData data)
        {
            if (data.model == null) return;

            //武器差し替え
            if (weaponModel != null) Destroy(weaponModel);

            currentWeapon = data;

            //武器生成・親設定
            weaponModel = Instantiate(data.model, transform.position, Quaternion.identity);

            //Debug.Log("handtype:" + data.handType + " / " + weaponModel.name, this);
            //手に持たせる
            switch (data.handType)
            {
                case HandType.None:
                    weaponModel.transform.SetParent(transform);
                    break;
                case HandType.Left:
                    weaponModel.transform.SetParent(handPos_L);
                    break;
                case HandType.Right:
                    weaponModel.transform.SetParent(handPos_R);
                    break;
            }

            //位置調整
            weaponModel.transform.localPosition = data.position;
            weaponModel.transform.localRotation = Quaternion.Euler(data.rotation);

            //コライダー無効化
            weaponCol = weaponModel.GetComponent<Collider>();
            weaponCol.enabled = false;
        }

        public void SetLayer(int layer = 0)
        {
            if (layer < 0) return;

            for (var i = 0; i < anim.layerCount; i++)
            {
                anim.SetLayerWeight(i, 0);
            }
            anim.SetLayerWeight(layer, 1);

            //anim.Play("Locomotion");
        }

        public void ShowCollider()
        {
            if (weaponModel == null)
            {
                Debug.Log($"<{transform.root.name}>コライダーが設定されていません", this);
                return;
            }

            weaponCol.enabled = !weaponCol.enabled;
        }

        public void SpawnBullet()
        {
            var gunData = currentWeapon as GunWeaponData;
            if (gunData is null) return;

            for (var i = 0; i < gunData.bulletCount; i++)
            {
                var bullet = Instantiate(gunData.bulletObj, weaponModel.transform.position, weaponModel.transform.rotation);
                bullet.GetComponent<CharacterStatus>().atk = gunData.atk;
                Destroy(bullet, 2);

                //rootのGameObjectが向いている方向に飛ばす
                var dir = transform.root.forward * (gunData.bulletSpeed * 10);

                if (gunData.bulletCount > 1)
                {
                    //角度を変える
                    //向いている方向を基準に30度の範囲で弾を発射する
                    var angle = Quaternion.Euler(0, Random.Range(-30, 30), 0);
                    //var angle = Quaternion.Euler(0, 360 / gunData.bulletCount * i, 0);
                    dir = angle * dir;
                }
                bullet.GetComponent<Rigidbody>().AddForce(dir);
            }
        }
    }
}