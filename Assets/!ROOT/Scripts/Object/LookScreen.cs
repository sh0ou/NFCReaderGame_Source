using UnityEngine;

namespace Jubatus
{
    /// <summary>
    /// オブジェクトを画面の方向に向ける
    /// </summary>
    public class LookScreen : MonoBehaviour
    {
        private void Update()
        {
            //カメラの方向を向く
            transform.LookAt(Camera.main.transform);
        }
    }
}