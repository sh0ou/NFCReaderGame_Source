using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

public class DeviceChecker : MonoBehaviour
{
    private ReadOnlyArray<InputDevice> devices;
    private string targetControllerName = "JC-W01U";

    [SerializeField] private Image image_controller, image_smartcard;
    [SerializeField] private Sprite sprite_controller_on, sprite_controller_off, sprite_smartcard_on, sprite_smartcard_off;
    private bool isReaderConnected = false;

    [SerializeField] public bool isActiveRead { get; set; }

    private AutoResetEvent waitEnqueueEvent = new(false);
    private bool isCardReaderRequestAlive;

    private void Awake()
    {
        //スプライトの初期化
        image_controller.sprite = sprite_controller_off;
        image_smartcard.sprite = sprite_smartcard_off;
        isActiveRead = true;

        //カードリーダーの接続状態を確認するスレッドを起動
        isCardReaderRequestAlive = true;

        _ = Task.Run(() =>
        {
            while (isCardReaderRequestAlive)
            {
                if (isActiveRead)
                {
                    isReaderConnected = CheckCardReader();
                }
                waitEnqueueEvent.WaitOne(100);
            }
        });
    }

    private void Start() { }

    private void Update()
    {
        //ゲームパッドの接続状態
        if (CheckController())
        {
            image_controller.sprite = sprite_controller_on;
            image_controller.color = Color.white;
        }
        else
        {
            image_controller.sprite = sprite_controller_off;
            image_controller.color = Color.red;
        }

        //カードリーダーの接続状態
        if (isReaderConnected)
        {
            image_smartcard.sprite = sprite_smartcard_on;
            image_smartcard.color = Color.white;
        }
        else
        {
            image_smartcard.sprite = sprite_smartcard_off;
            image_smartcard.color = Color.red;
        }
    }

    private void OnDestroy()
    {
        isCardReaderRequestAlive = false;
    }

    private const string DLL_NAME_FELICA = "felicalib64.dll";
    #region dll
    [DllImport(DLL_NAME_FELICA)] private extern static IntPtr pasori_open(String dummy);
    [DllImport(DLL_NAME_FELICA)] private extern static void pasori_close(IntPtr pasori);
    [DllImport(DLL_NAME_FELICA)] private extern static int pasori_init(IntPtr pasori);
    #endregion

    /// <summary> カードリーダーの接続状態を確認する </summary>
    /// <returns></returns>
    private bool CheckCardReader()
    {
        var pasoriP = pasori_open(null);
        if (pasoriP == IntPtr.Zero)
        {
            Debug.LogError($"{DLL_NAME_FELICA}を開けません");
            return false;
        }
        if (pasori_init(pasoriP) != 0)
        {
            //Debug.LogWarning("PaSoRiに接続できません");
            pasori_close(pasoriP);
            return false;
        }
        pasori_close(pasoriP);
        return true;
    }

    /// <summary> コントローラーの接続状態を確認する </summary>
    /// <returns></returns>
    private bool CheckController()
    {
        // コントローラーの接続状態を確認する
        devices = InputSystem.devices;

        foreach (var controller in devices)
        {
            if (controller.name.Contains(targetControllerName))
            {
                return true;
            }
        }

        return false;
    }
}
