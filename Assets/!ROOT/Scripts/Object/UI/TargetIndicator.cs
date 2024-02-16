using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jubatus
{
    public class TargetIndicator : MonoBehaviour
    {
        public Transform playerTransform;
        public Image enemyIcon;
        public RectTransform iconParent;
        public float borderSize = 50f;

        public List<Transform> enemies;

        private Transform enemyTransform;
        private TextMeshProUGUI enemyIconText;

        private void Awake()
        {
            enemyIconText = enemyIcon.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Update()
        {
            var closestDistance = Mathf.Infinity;

            enemies.Clear();
            foreach (var enemyLoco in FindObjectsByType<EnemyLocomotion>(FindObjectsSortMode.None))
            {
                enemies.Add(enemyLoco.transform);
            }

            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(playerTransform.position, enemy.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    enemyTransform = enemy;
                }
            }

            if (enemyTransform != null)
            {
                var enemyPos = new Vector3(enemyTransform.position.x, enemyTransform.position.y, enemyTransform.position.z);
                var dir = (enemyPos - playerTransform.position).normalized;

                var enemyViewportPos = Camera.main.WorldToViewportPoint(enemyPos);
                if (enemyViewportPos.z > 0f &&
                    enemyViewportPos.x >= 0f &&
                    enemyViewportPos.x <= 1f &&
                    enemyViewportPos.y >= 0f &&
                    enemyViewportPos.y <= 1f)
                {
                    enemyIcon.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    enemyIcon.gameObject.SetActive(true);

                    var enemyScreenPos = Camera.main.WorldToScreenPoint(enemyPos);

                    var cappedPos = enemyScreenPos;
                    if (enemyScreenPos.x <= borderSize) cappedPos.x = borderSize;
                    if (enemyScreenPos.x >= Screen.width - borderSize) cappedPos.x = Screen.width - borderSize;
                    if (enemyScreenPos.y <= borderSize) cappedPos.y = borderSize;
                    if (enemyScreenPos.y >= Screen.height - borderSize) cappedPos.y = Screen.height - borderSize;

                    //アイコンの位置を更新
                    enemyIcon.rectTransform.position = cappedPos;

                    //アイコンの向きを敵の方向に向ける
                    var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    enemyIcon.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);

                    UpdateText(closestDistance);
                }
            }
            else
            {
                enemyIcon.gameObject.SetActive(false);
            }
        }

        private void UpdateText(float distance)
        {
            //Y軸が一定以上の場合、Y軸の差の値を表示
            if (Mathf.Abs(enemyTransform.position.y - playerTransform.position.y) > 1f)
            {
                //値がプラス=青 、マイナス=赤
                enemyIconText.text = enemyTransform.position.y - playerTransform.position.y > 0f
                    ? $"Y:<color=blue>+{enemyTransform.position.y - playerTransform.position.y:F1}</color>m"
                    : $"Y:<color=red>{enemyTransform.position.y - playerTransform.position.y:F1}</color>m";
            }
            else
            {
                //距離を表示
                enemyIconText.text = $"{distance:F1}m";
            }

            enemyIconText.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}