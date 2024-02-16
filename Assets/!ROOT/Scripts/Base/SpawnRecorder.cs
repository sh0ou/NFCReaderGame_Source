using UnityEditor;
using UnityEngine;

namespace Jubatus
{
    /// <summary> シーン上にいる敵のスポーン設定を記録する </summary>
    public class SpawnRecorder : MonoBehaviour
    {
        [SerializeField, Label("Start時に記録")] private bool isRecordOnStart = true;
        [SerializeField, Label("記録先")] private EnemySpawnDataList spawnDataList;
        [SerializeField, Label("記録完了時に再生を停止")] private bool isStopPlay = true;

        private void Start()
        {
            if (isRecordOnStart)
                Record();
        }

        private void Record()
        {
            var enemyObjs = GetComponentsInChildren<SpawnRecordTarget>();
            var tempList = spawnDataList.enemySpawnDatas;

            spawnDataList.enemySpawnDatas = new EnemySpawnData[enemyObjs.Length];
            for (var i = 0; i < enemyObjs.Length; i++)
            {
                Debug.Log($"出力中... : {i + 1}/{enemyObjs.Length}", this);
                var enemyObj = enemyObjs[i];

                if (enemyObj.prefab != null)
                {
                    spawnDataList.enemySpawnDatas[i] = new EnemySpawnData();
                    spawnDataList.enemySpawnDatas[i].prefab = enemyObj.prefab;
                    spawnDataList.enemySpawnDatas[i].spawnPos = enemyObj.transform.position;
                }
                else
                {
                    Debug.LogError($"出力に失敗しました : {i + 1}/{enemyObjs.Length} - {enemyObj.name}", this);
                    spawnDataList.enemySpawnDatas = new EnemySpawnData[tempList.Length];
                    spawnDataList.enemySpawnDatas = tempList;
                    return;
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(spawnDataList);
            AssetDatabase.SaveAssets();
#endif

            Debug.Log("出力完了！", this);

            //エディタの再生を停止
#if UNITY_EDITOR
            if (isStopPlay)
                UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}