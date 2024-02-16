using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Jubatus
{
    public class TokenStorage : MonoBehaviour
    {
        [SerializeField] private Animator[] anims;
        [SerializeField] private GameObject tokenUIroot;
        [SerializeField] public GameObject tokenObj;
        [SerializeField] private Image[] tokenUIs;
        [SerializeField, UnEditable] private TokenCat[] currentTokens;

        [SerializeField] private Image[] useTokenCountUIs;

        private string animName_add = "Add", animName_use = "Use";

        public enum TokenCat
        {
            None,
            Red,
            Blue,
            Green,
            X //ワイルドカード
        };

        private void Start()
        {
            //トークンUI郡を取得
            var uis = tokenUIroot.GetComponentsInChildren<Image>();
            tokenUIs = new Image[uis.Length];
            tokenUIs = uis;
            anims = new Animator[uis.Length];
            currentTokens = new TokenCat[uis.Length];
            foreach (var useUi in useTokenCountUIs)
            {
                useUi.enabled = false;
            }

            ResetToken(uis.Length, true);
        }

        private void Update()
        {

        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="cat">トークンカテゴリ</param>
        public bool AddToken(TokenCat cat)
        {
            //左からスタックしていく
            for (var i = 0; i < tokenUIs.Length; i++)
            {
                if (currentTokens[i] == TokenCat.None)
                {
                    currentTokens[i] = cat;
                    tokenUIs[i].color = GetColor(currentTokens[i]);
                    anims[i].SetTrigger(animName_add);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// トークンを使用<br/>使用できない場合はfalseを返す
        /// </summary>
        /// <param name="cat">トークンカラー</param>
        /// <param name="count">使用する数</param>
        public bool UseToken(TokenCat cat, int count)
        {
            //トークンを使用しない場合はtrueを返す
            if (cat == TokenCat.None) return true;

            //配列から使用するトークンカラーを取得
            TokenCat[] tokens = currentTokens.Where(t => t == cat || t == TokenCat.X).ToArray();

            //トークン数が足りているか確認
            if (tokens.Length >= count)
            {
                var usedCount = 0;  //使用したトークンの数

                //足りていればトークンを消す
                for (var i = 0; i < currentTokens.Length; i++)
                {
                    if ((currentTokens[i] == cat || currentTokens[i] == TokenCat.X) && usedCount < count)
                    {
                        //トークンを消す
                        currentTokens[i] = TokenCat.None;
                        tokenUIs[i].color = GetColor(TokenCat.None);

                        //アニメーション再生
                        var str = "";
                        switch (cat) { case TokenCat.Red: str = "_R"; break; case TokenCat.Green: str = "_G"; break; case TokenCat.Blue: str = "_B"; break; }
                        anims[i].SetTrigger($"{animName_use}{str}");
                        usedCount++;
                    }
                }

                //トークンを左に詰める
                var currentIndex = 0;
                for (var i = 0; i < currentTokens.Length - 1; i++)
                {
                    if (currentTokens[i] != TokenCat.None)
                    {
                        currentTokens[currentIndex] = currentTokens[i];
                        tokenUIs[currentIndex].color = GetColor(currentTokens[i]);
                        currentIndex++;
                    }
                }

                //残りのトークンをNoneにする
                for (var i = currentIndex; i < currentTokens.Length; i++)
                {
                    currentTokens[i] = TokenCat.None;
                    tokenUIs[i].color = GetColor(TokenCat.None);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// トークンをリセット
        /// </summary>
        /// <param name="uiCount">トークンUIの数</param>
        public void ResetToken(int uiCount, bool isSetAnimator = false)
        {
            for (var i = 0; i < uiCount; i++)
            {
                currentTokens[i] = TokenCat.None;
                tokenUIs[i].color = GetColor(currentTokens[i]);
                if (isSetAnimator) anims[i] = tokenUIs[i].GetComponent<Animator>();
                anims[i].Play("Idle", 0);
            }
        }

        /// <summary>
        /// 各トークンの色を取得
        /// </summary>
        /// <param name="cat">トークンカテゴリ</param>
        /// <returns></returns>
        private Color GetColor(TokenCat cat)
        {
            switch (cat)
            {
                case TokenCat.Red:
                    return Color.red;
                case TokenCat.Blue:
                    return Color.blue;
                case TokenCat.Green:
                    return Color.green;
                case TokenCat.X:
                    return Color.white;
                default:
                    return Color.gray;
            }
        }

        public void SetUseTokenCount(int count, TokenCat cat)
        {
            //トークン使用数UIをすべて非表示にする
            foreach (var ui in useTokenCountUIs)
            {
                ui.enabled = false;
            }

            //countの数だけトークン使用数UIを表示する
            for (var i = 0; i < count; i++)
            {
                useTokenCountUIs[i].enabled = true;
                useTokenCountUIs[i].color = GetColor(cat);
            }
        }
    }
}