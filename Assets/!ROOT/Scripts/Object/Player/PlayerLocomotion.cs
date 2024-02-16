using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jubatus
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [SerializeField] private GameObject modelRoot;
        [SerializeField] private PlayerData playerData;

        public bool isCanAction { get; set; }
        [SerializeField, Label("移動可能か？")] private bool isCanMove = true;

        [SerializeField, UnEditable] private Vector2 moveVector;
        [SerializeField, Label("移動スピード")] private int moveSpeed = 5;
        [SerializeField, Label("回避可能か？")] private bool isCanDodge = true;

        #region VarNameList
        [SerializeField, Label("アクション名 - 移動"), UnEditable] private string actionName_Move = "Move";
        [SerializeField, Label("アクション名 - 回避"), UnEditable] private string actionName_Dodge = "Dodge";
        [SerializeField, Label("アクション名 - 攻撃"), UnEditable] private string actionName_Attack = "Attack";

        [SerializeField, Label("Anim変数名 - 移動Blend")] private string animName_Move;
        [SerializeField, Label("Anim変数名 - 攻撃")] private string animName_Attack;
        [SerializeField, Label("Anim変数名 - 被弾")] private string animName_Damage;
        [SerializeField, Label("Anim変数名 - 回避")] private string animName_Dodge;
        #endregion

        private Animator anim;
        private Rigidbody rb;
        private PlayerInput playerInput;
        private CharacterAction charaAction;
        private CharacterStatus charaStatus;
        private TokenStorage tokenStorage;
        [SerializeField] private WeaponAttach weaponAttach;

        private SoundPlayer sePlayer;

        private Vector3 latestPos;

        private void Awake()
        {
            //初期化
            anim = modelRoot.GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();
            charaAction = GetComponent<CharacterAction>();
            charaStatus = GetComponent<CharacterStatus>();
            tokenStorage = GetComponent<TokenStorage>();
            sePlayer = GameObject.Find("SEPlayer").GetComponent<SoundPlayer>();

            isCanAction = false;
        }

        private void Update()
        {
            //攻撃時は押し出しを禁止
            rb.constraints = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == animName_Attack
                ? RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation
                : rb.constraints = RigidbodyConstraints.FreezeRotation;

            //落下時はリスポーン
            if (transform.position.y < -10)
            {
                transform.position = Vector3.one;
            }
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void OnEnable()
        {
            if (TryGetComponent<PlayerInput>(out var component))
            {
                playerInput = component;
                //アクションの登録
                playerInput.actions[actionName_Move].performed += OnMove;
                playerInput.actions[actionName_Move].canceled += OnMove;
                playerInput.actions[actionName_Dodge].performed += OnDodge;
                playerInput.actions[actionName_Attack].performed += OnAttack;
            }
            else { return; }
        }

        private void OnDisable()
        {
            if (TryGetComponent<PlayerInput>(out var component))
            {
                playerInput = component;
                //アクションの登録解除
                playerInput.actions[actionName_Move].performed -= OnMove;
                playerInput.actions[actionName_Move].canceled -= OnMove;
                playerInput.actions[actionName_Dodge].performed -= OnDodge;
                playerInput.actions[actionName_Attack].performed -= OnAttack;
            }
            else { return; }
        }

        //todo:他スクリプトにトークン取得処理を作成

        private void Move()
        {
            //移動処理
            rb.MovePosition(transform.position + new Vector3(moveVector.x, 0, moveVector.y) * moveSpeed * Time.deltaTime);
            if (moveVector == Vector2.zero) rb.velocity = new Vector3(0, rb.velocity.y, 0);

            //移動方向に向く
            var diff = transform.position - latestPos;
            latestPos = transform.position;
            if (diff.magnitude > 0.01f)
            {
                //leapで少しずつ回転させる（x,z軸は固定）
                var rotation = Quaternion.LookRotation(new Vector3(diff.x, 0, diff.z));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 360 * (Time.deltaTime * 2));
            }

            //移動アニメーション
            var animMove = anim.GetFloat(animName_Move);
            var animMove_after = moveVector == Vector2.zero ? animMove - 0.1f : animMove + 0.1f;
            //blendの値を変更
            anim.SetFloat(animName_Move, Mathf.Clamp(animMove_after, 0, 1));
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            //静止時
            if (context.canceled)
            {
                moveVector = Vector2.zero;
                return;
            }

            if (!isCanAction || !isCanMove || charaStatus.invincibleTime > 0) return;
            //移動方向を取得
            moveVector = context.ReadValue<Vector2>();
        }

        private void OnDodge(InputAction.CallbackContext context)
        {
            if (!isCanAction || !isCanDodge || charaStatus.invincibleTime > 0) return;

            Debug.Log(nameof(OnDodge), this);
            anim.SetTrigger(animName_Dodge);
            sePlayer.PlaySE(5);
            charaStatus.invincibleTime = 0.5f;
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (!isCanAction || charaStatus.invincibleTime > 0) return;

            Debug.Log(nameof(OnAttack), this);
            if (charaStatus.weaponData.color == TokenStorage.TokenCat.None
                || tokenStorage.UseToken(charaStatus.weaponData.color, charaStatus.weaponData.cost))
            {
                switch (charaStatus.weaponData.type)
                {
                    case WeaponType.Item_Heal:
                        charaAction.Heal(charaStatus.weaponData.atk);
                        break;
                    case WeaponType.AKGun:
                        anim.SetTrigger(animName_Attack);
                        sePlayer.PlaySE(0);
                        break;
                    default:
                        anim.SetTrigger(animName_Attack);
                        break;
                }
            }
        }

        private void OnReadCard(CardData data)
        {
            charaStatus.weaponData = data.weaponData;

            //武器切替
            weaponAttach.SetWeapon(data.weaponData);
        }

        public Animator GetModelAnimator() => anim;
    }
}