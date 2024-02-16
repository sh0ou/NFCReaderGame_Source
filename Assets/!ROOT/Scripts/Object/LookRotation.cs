using UnityEngine;

namespace Jubatus
{
    public class LookRotation : MonoBehaviour
    {
        [SerializeField, Label("向く方向のObj")] private GameObject target;

        private void Update()
        {
            // 対象物と自分自身の座標からベクトルを算出
            Vector3 vector3 = target.transform.position - this.transform.position;
            // 上下方向の回転を制限
            vector3.y = 0f;

            // 回転値を算出し、このGameObjectのrotationに代入
            var quaternion = Quaternion.LookRotation(vector3);
            this.transform.rotation = quaternion;
        }
    }
}