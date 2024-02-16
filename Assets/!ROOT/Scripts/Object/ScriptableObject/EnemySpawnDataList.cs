using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jubatus
{
    [CreateAssetMenu(fileName = "EnemySpawnDataList", menuName = "ScriptableObject/EnemySpawnDataList")]
    public class EnemySpawnDataList : ScriptableObject
    {
        public string sceneName;
        public EnemySpawnData[] enemySpawnDatas;
    }

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject prefab;
        public Vector3 spawnPos; //生成位置
    }
}