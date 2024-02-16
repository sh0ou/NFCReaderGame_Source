using System;
using UnityEngine;
using UnityEngine.AI;

namespace Jubatus
{
    public class EnemyLocomotion : MonoBehaviour
    {
        [SerializeField] public CharacterData status;
        [SerializeField] public EnemyData enemyData;

        [SerializeField] private Animator anim;
        [SerializeField, Label("攻撃範囲")] private Collider actionArea;
        private Transform followTarget;
        private NavMeshAgent agent;
        private bool isAttack;
        public bool isCanMove { get; set; }

        private TokenStorage tokenStorage;
        private TokenStorage.TokenCat[] tokenValues;

        #region VarNameList
        [SerializeField, Label("Anim変数名 - 移動Blend")] private string animName_Move;
        [SerializeField, Label("Anim変数名 - 攻撃")] private string animName_Attack;
        [SerializeField, Label("Anim変数名 - 被弾")] private string animName_Damage;
        [SerializeField, Label("Anim変数名 - 倒れ")] private string animName_Dead;
        #endregion

        private void Awake()
        {
            //初期化
            agent = GetComponent<NavMeshAgent>();
            followTarget = FindAnyObjectByType<PlayerLocomotion>().transform;
            tokenStorage = FindAnyObjectByType<TokenStorage>();
            tokenValues = (TokenStorage.TokenCat[])Enum.GetValues(typeof(TokenStorage.TokenCat));
            isCanMove = true;

            agent.speed = UnityEngine.Random.Range(status.moveSpeed * 0.7f, status.moveSpeed * 1.2f);
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (!isCanMove)
            {
                anim.SetFloat(animName_Move, 0);
                anim.SetBool(animName_Attack, false);
                return;
            }

            if (followTarget != null)
            {
                if (isAttack)
                {
                    //攻撃
                    anim.SetFloat(animName_Move, 0);
                    anim.SetBool(animName_Attack, true);
                    //if (isTrack)
                    agent.SetDestination(transform.position);
                }
                else
                {
                    //追跡
                    anim.SetFloat(animName_Move, 1);
                    anim.SetBool(animName_Attack, false);
                    agent.SetDestination(followTarget.position);
                }
            }
        }

        public void GenerateToken()
        {
            //トークン生成
            var random = tokenValues[UnityEngine.Random.Range(0, tokenValues.Length)];
            var pos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            var obj = Instantiate(tokenStorage.tokenObj, pos, Quaternion.identity);
            Destroy(obj, 30);
            switch (random)
            {
                case TokenStorage.TokenCat.Red:
                    obj.gameObject.tag = "Token_R";
                    obj.GetComponent<Animator>().Play("Red");
                    break;
                case TokenStorage.TokenCat.Blue:
                    obj.gameObject.tag = "Token_B";
                    obj.GetComponent<Animator>().Play("Blue");
                    break;
                case TokenStorage.TokenCat.Green:
                    obj.gameObject.tag = "Token_G";
                    obj.GetComponent<Animator>().Play("Green");
                    break;
                case TokenStorage.TokenCat.X:
                    obj.gameObject.tag = "Token_X";
                    obj.GetComponent<Animator>().Play("Wild");
                    break;
                default:
                    Destroy(obj);
                    break;
            }
            if (obj.TryGetComponent<Rigidbody>(out Rigidbody r))
            {
                //vector3.left等方向をランダムで決める
                var randomVector = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));

                r.AddForce((Vector3.up + randomVector) * UnityEngine.Random.Range(2, 4), ForceMode.Impulse);
            }
        }

        public void ChangeAttackFlag(bool flag) => isAttack = flag;
    }
}