using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using FelicaLib;
using System.Collections;

namespace Jubatus
{
    public class SmartCardReader : MonoBehaviour
    {
        public bool isActiveRead { get; set; }
        [SerializeField] private SystemCode targetSystem;
        [SerializeField, UnEditable] public string currentCardIdm;
        [SerializeField, UnEditable] public SystemCode currentSystemCode;

        private Dictionary<SystemCode, ServiceCode> systemCodeServiceCodeMap;

        // スレッド関係
        public Mutex currentValuesMtx = new();// cardIdm変数を複数のスレッドからアクセスするため保護する
        private AutoResetEvent waitEnqueueEvent = new(false);
        private bool isCardReaderRequestAlive;
        private Mutex isCardReaderRequestAliveMtx = new();// isCardReaderRequestAlive変数を複数のスレッドからアクセスするため保護する

        [SerializeField] private CardSwitcher cardSwitcher;

        private void Awake()
        {
            GenerateCodeEnumMap();

            isCardReaderRequestAliveMtx.WaitOne();
            isCardReaderRequestAlive = true;
            isCardReaderRequestAliveMtx.ReleaseMutex();

            isActiveRead = false;

            // 非同期で無限ループを回す
            // 参考：https://qiita.com/inew/items/0126270bca99883605de#3-%E9%9D%9E%E5%90%8C%E6%9C%9F%E3%81%A7%E7%84%A1%E9%99%90%E3%83%AB%E3%83%BC%E3%83%97%E3%82%92%E5%9B%9E%E3%81%99
            _ = Task.Run(async () =>
            {
                // 100ms以上の周期でカードリーダからデータを読み取り更新する
                while (true)
                {
                    if (!isCardReaderRequestAlive) break;

                    if (isActiveRead)
                    {
                        try
                        {
                            using (var f = new Felica())
                            {
                                var (tempIdm, tempSysCode) = ReadCard(f, targetSystem);
                                // cardIdmにアクセスOKになるまで待つ
                                currentValuesMtx.WaitOne();
                                //IDmが変わったらイベントを発火する
                                if (tempIdm != currentCardIdm)
                                {
                                    currentCardIdm = tempIdm;
                                    currentSystemCode = tempSysCode;
                                }
                                // cardIdmのアクセスを、他のスレッドが使って良いことを宣言する
                                currentValuesMtx.ReleaseMutex();
                            }
                        }
                        catch (Exception)
                        {
                            currentValuesMtx.WaitOne();
                            currentSystemCode = SystemCode.None;
                            currentCardIdm = "";

                            currentValuesMtx.ReleaseMutex();
                        }

                    }
                    await Task.Delay(100);// 100ms待つ
                }
            });
        }

        private void OnDestroy()
        {
            isCardReaderRequestAliveMtx.WaitOne();
            isCardReaderRequestAlive = false;
            isCardReaderRequestAliveMtx.ReleaseMutex();
        }

        private void GenerateCodeEnumMap()
        {
            systemCodeServiceCodeMap = new Dictionary<SystemCode, ServiceCode>();
            foreach (SystemCode sysCode in Enum.GetValues(typeof(SystemCode)))
            {
                var svcCode = GetServiceCodeForSystemCode(sysCode);
                systemCodeServiceCodeMap[sysCode] = svcCode;
            }
        }

        private ServiceCode GetServiceCodeForSystemCode(SystemCode s)
        {
            switch (s)
            {
                case SystemCode.None: return ServiceCode.None;
                case SystemCode.Any: return ServiceCode.Any;
                case SystemCode.Common: return ServiceCode.None;    // 未実装
                case SystemCode.Cyberne: return ServiceCode.Cyberne;
                case SystemCode.FelicaLiteS: return ServiceCode.FelicaLiteS;
                case SystemCode.FelicaStandard: return ServiceCode.FelicaStandard;
                case SystemCode.ANA: return ServiceCode.None;       // 未実装
                case SystemCode.JAL: return ServiceCode.None;       // 未実装
                case SystemCode.Sapica: return ServiceCode.Sapica;
                default: return ServiceCode.None;
            }
        }

        private (string, SystemCode) ReadCard(Felica f, SystemCode sysc)
        {
            //カードを読み取る
            f.Polling((int)sysc);

            if (systemCodeServiceCodeMap.TryGetValue(sysc, out var svcc))
            {
                var data = f.ReadWithoutEncryption((int)svcc, 0);
                if (data == null)
                {
                    //Debug.LogWarning("IDが読み取れませんでした");
                    return ("", SystemCode.None);
                }

                var str = "";
                for (var i = 0; i < 8; i++)
                {
                    str += f.IDm()[i].ToString("X2");
                }

                if (str != "")
                {
                    currentSystemCode = sysc;
                    return (str, sysc);
                }
                else
                {
                    currentSystemCode = SystemCode.None;
                    return ("", SystemCode.None);
                }
            }
            else
            {
                // サービスコードが見つからなかった場合
                return (currentCardIdm, currentSystemCode);
            }
        }
    }
}