using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Jubatus
{
    public class CharacterAction : MonoBehaviour
    {
        [SerializeField] public CharacterStatus status;
        [SerializeField, UnEditable] private Material[] mats;//無敵時点滅させるマテリアル
        [SerializeField] private GameObject damageEffect;

        [SerializeField, Header("HP切れイベント")] private UnityEvent ev_Die;

        private bool isScored = false;
        private ScoreCounter scoreCounter;
        private EnemyLocomotion enemyLoco;

        private SoundPlayer sePlayer;

        private string tagName_player = "Player";
        private string tagName_enemy = "Enemy";

        private void Awake()
        {
            mats = GetComponentsInChildren<Renderer>().Distinct().Select(x => x.material).ToArray();
            sePlayer = GameObject.Find("SEPlayer").GetComponent<SoundPlayer>();
            if (gameObject.tag == tagName_enemy)
            {
                enemyLoco = GetComponent<EnemyLocomotion>();
                scoreCounter = FindAnyObjectByType<ScoreCounter>();
            }
        }

        private void Update()
        {
            if (status.invincibleTime > 0)
            {
                status.invincibleTime -= Time.deltaTime;
                foreach (var m in mats)
                {
                    m.color = new Color(m.color.r, m.color.g, m.color.b, Mathf.PingPong(Time.time * 10, 1));
                }
            }
            else
            {
                status.invincibleTime = 0;
                foreach (var m in mats)
                {
                    m.color = new Color(m.color.r, m.color.g, m.color.b, 1);
                }
            }
        }

        public void OnChangeHP(int value)
        {
            //マイナス値の場合
            if (Mathf.Sign(value) == -1)
            {
                Damage(value);
            }
            else
            {
                Heal(value);
            }
        }

        /// <summary> ダメージを受ける </summary>
        /// <param name="damage"></param>
        public void Damage(int damage)
        {
            if (status.invincibleTime > 0) return;

            //Debug.Log($"Damage! / {transform.root.name}", this);
            //ダメージ値処理
            status.hp -= damage;
            var effect = Instantiate(damageEffect, transform.root.position + damageEffect.transform.position, damageEffect.transform.rotation);
            Destroy(effect, 1.5f);

            if (gameObject.tag == tagName_enemy)
            {
                sePlayer.PlaySE(3);
            }
            else if (gameObject.tag == tagName_player)
            {
                sePlayer.PlaySE(4);
                status.invincibleTime = 0.5f;
            }

            if (status.hp <= 0)
            {
                status.hp = 0;
                ev_Die.Invoke();

                if (gameObject.tag == tagName_enemy)
                {
                    enemyLoco.GenerateToken();

                    //該当する敵をリストから削除
                    var spawnner = FindAnyObjectByType<EnemySpawnner>();
                    spawnner.RemoveEnemy(gameObject);
                    if (!isScored)
                    {
                        isScored = true;
                        scoreCounter.AddScore(enemyLoco.enemyData.score);
                        scoreCounter.SendScore();
                    }
                }
            }
            else
            {
                if (gameObject.tag == tagName_enemy)
                {
                    //スコア加算
                    scoreCounter.AddScore(enemyLoco.enemyData.score / 10);
                    enemyLoco.GenerateToken();
                }
            }
        }

        public void Heal(int heal)
        {
            status.hp += heal;
            if (status.hp > status.maxHp)
            {
                status.hp = status.maxHp;
            }
            sePlayer.PlaySE(12);
        }

        public void ObjDestroy(float destroyTime = 0) => Destroy(transform.root.gameObject, destroyTime);
    }
}