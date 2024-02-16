using FelicaLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jubatus
{
    public class CardSwitcher : MonoBehaviour
    {
        [SerializeField] private SmartCardReader reader;
        [SerializeField] private WeaponAttach weaponAttach;
        private CharacterStatus charaStatus;
        private TokenStorage tokenStorage;
        [SerializeField] private Image cardUI;

        [Header("カード設定")]
        [SerializeField] private CardData[] cardDatas;
        [SerializeField] private CardData defaultCardData;
        private Dictionary<string, CardData> cardDataDict = new();
        private CardData tmpCard;
        private SoundPlayer sePlayer;

        private void Start()
        {
            charaStatus = FindAnyObjectByType<PlayerLocomotion>().GetComponent<CharacterStatus>();
            tokenStorage = FindAnyObjectByType<PlayerLocomotion>().GetComponent<TokenStorage>();
            sePlayer = GameObject.Find("SEPlayer").GetComponent<SoundPlayer>();

            //配列を辞書に変換
            cardDataDict.Clear();
            for (var i = 0; i < cardDatas.Length; i++)
            {
                cardDataDict.Add(cardDatas[i].id, cardDatas[i]);
            }

            tmpCard = defaultCardData;
            weaponAttach.SetWeapon(defaultCardData.weaponData);
            weaponAttach.SetLayer((int)defaultCardData.weaponData.type);
        }

        private void Update()
        {
            CheckCard(reader.currentCardIdm);
        }

        public void CheckCard(string idm)
        {
            //Debug.Log(idm, this);
            //デフォルトのカード
            if (idm == "")
            {
                SetCard(defaultCardData);
                return;
            }

            SystemCode currentSystemCodeCopy;

            reader.currentValuesMtx.WaitOne();
            currentSystemCodeCopy = reader.currentSystemCode;
            reader.currentValuesMtx.ReleaseMutex();

            foreach (var card in cardDataDict)
            {
                //Debug.Log($"cardsys:{card.Value.systemCode}", this);
                if (currentSystemCodeCopy != card.Value.systemCode) continue;

                //Debug.Log($"isAll:{card.Value.isAllowAllIDm} / key:{card.Key} / id:{card.Value.id}", this);
                if (card.Value.isAllowAllIDm)
                {
                    //読み取ったカード
                    SetCard(card.Value);
                    return;
                }
                else
                {
                    if (idm == card.Key)
                    {
                        //読み取ったカード
                        SetCard(card.Value);
                        return;
                    }
                }
            }

            //デフォルトのカード
            SetCard(defaultCardData);
        }

        private void SetCard(CardData c)
        {
            if (c == tmpCard) return;
            tmpCard = c;
            Debug.Log($"カードセット！:{weaponAttach.GetWeaponModel()} >> {c.weaponData.model}", this);
            if (c.weaponData.model == null || weaponAttach.GetWeaponModel() == c.weaponData.model) return;

            Debug.Log("モデル変更", this);
            cardUI.sprite = c.sprite;
            charaStatus.weaponData = c.weaponData;
            charaStatus.atk = charaStatus.characterData.atk + c.weaponData.atk;
            tokenStorage.SetUseTokenCount(c.weaponData.cost, c.weaponData.color);

            weaponAttach.SetWeapon(c.weaponData);
            weaponAttach.SetLayer((int)c.weaponData.type);

            sePlayer.PlaySE(2);
        }
    }
}