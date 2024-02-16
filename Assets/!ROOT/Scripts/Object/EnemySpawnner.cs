using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Jubatus
{
    public class EnemySpawnner : MonoBehaviour
    {
        [SerializeField, Header("敵生成データ")] private EnemySpawnDataList[] spawnDatas;
        [SerializeField, Header("敵全滅時")] private UnityEvent onWipedOut;
        [SerializeField, Header("最終ウェーブクリア時")] private UnityEvent onLastWaveClear;
        [SerializeField, UnEditable] private int currentSpawnListIndex = 0;  //現在のスポーン番号
        [SerializeField, UnEditable] private List<GameObject> fieldObjList;  //フィールド上の敵リスト

        [SerializeField] private TextMeshProUGUI text_wave, text_enemyCount;
        [SerializeField] private Image countGauge;

        private void Start()
        {
            //SpawnEnemy(0);
            text_wave.text = $"----";
            text_enemyCount.text = $"----";
        }

        /// <summary>
        /// 指定したリスト番号の敵を生成する
        /// </summary>
        /// <param name="index">リスト番号<br/>-1= 次のリストを選択<br/>-2= 同じリストを選択</param>
        public void SpawnEnemy(int index)
        {
            //スポーン番号の更新
            switch (index)
            {
                case -1:
                    currentSpawnListIndex++;
                    Debug.Log("next", this);
                    break;
                case -2:
                    break;
                default:
                    currentSpawnListIndex = index;
                    break;
            }

            //最終ウェーブをクリアしたら終了
            if (currentSpawnListIndex >= spawnDatas.Length)
            {
                currentSpawnListIndex--;

                //UI更新
                text_wave.text = $"<size=56>CLEARED</size>";
                text_enemyCount.text = $"Last:--";
                countGauge.fillAmount = 0;

                //イベント発火
                onLastWaveClear.Invoke();
            }
            else
            {
                //敵を生成
                foreach (var spawnData in spawnDatas[currentSpawnListIndex].enemySpawnDatas)
                {
                    var obj = Instantiate(spawnData.prefab, spawnData.spawnPos, Quaternion.identity);
                    fieldObjList.Add(obj);
                }

                //UI更新
                text_wave.text = $"Wave <size=60>{currentSpawnListIndex + 1}</size>";
                countGauge.fillAmount = 1;
                text_enemyCount.text = $"Last:{fieldObjList.Count}";
            }
        }

        /// <summary>
        /// 敵カウントを減らす
        /// </summary>
        public void RemoveEnemy(GameObject obj)
        {
            if (fieldObjList.Count == 0) return;

            //UI更新
            fieldObjList.Remove(obj);
            text_enemyCount.text = $"Last:{fieldObjList.Count}";
            countGauge.fillAmount = (float)fieldObjList.Count / spawnDatas[currentSpawnListIndex].enemySpawnDatas.Length;

            if (fieldObjList.Count == 0)
            {
                onWipedOut.Invoke();
            }
        }

        /// <summary>
        /// 指定したシーンに移動する
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(string sceneName)
        {
            //シーン名が違う場合のみ移動
            if (sceneName == "" || SceneManager.GetSceneAt(1).name == sceneName) return;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary> ウェーブ数を送信 </summary>
        public void SendWaveCount(bool isClear = false)
        {
            if (FindAnyObjectByType<ResultUI>().TryGetComponent<ResultUI>(out var result))
            {
                var str = isClear
                    ? $"Wave: {currentSpawnListIndex + 1}/{spawnDatas.Length} <size=16><color=yellow>ALL CLEAR!!</color></size>"
                    : $"Wave: {currentSpawnListIndex + 1}/{spawnDatas.Length}";
                result.SetResultInfo(ResultUI.ResultInfoType.Wave, str);
            }
        }

        public void ToggleEnemyMove(bool isCanMove)
        {
            foreach(var obj in fieldObjList)
            {
                obj.GetComponent<EnemyLocomotion>().isCanMove = isCanMove;
            }
        }
    }
}