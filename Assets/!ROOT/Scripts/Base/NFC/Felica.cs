using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FelicaLib
{
    //システムコード
    public enum SystemCode : int
    {
        None = 0x0000,          //None
        Any = 0xffff,           //Any
        Common = 0xfe00,        //共通領域
        Cyberne = 0x0003,       //サイバネ領域（交通系IC）

        FelicaLiteS = 0x88b4,   //Felica Lite-S
        FelicaStandard = 0x816a,//Felica Standard

        ANA = 0x8086,           //ANACard
        JAL = 0x816e,           //JALCard
        Sapica = 0x865e,        //Sapica
    }

    public enum ServiceCode : int
    {
        None = 0x0000,          //None
        Any = 0xffff,           //Any
        Cyberne = 0x090f,       //サイバネ領域（交通系IC）

        FelicaLiteS = 0x0009,   //Felica Lite-S
        FelicaStandard = 0x0109,//Felica Standard

        Sapica = 0x008b,        //Sapica
    }

    public class Felica : IDisposable
    {
        public const string DLL_NAME_FELICA = "felicalib64.dll";

        #region dll
        [DllImport(DLL_NAME_FELICA)] private extern static IntPtr pasori_open(String dummy);
        [DllImport(DLL_NAME_FELICA)] private extern static void pasori_close(IntPtr pasori);
        [DllImport(DLL_NAME_FELICA)] private extern static int pasori_init(IntPtr pasori);
        [DllImport(DLL_NAME_FELICA)] private extern static IntPtr felica_polling(IntPtr p, ushort sysCode, byte rfu, byte time_slot);
        [DllImport(DLL_NAME_FELICA)] private extern static void felica_free(IntPtr f);
        [DllImport(DLL_NAME_FELICA)] private extern static void felica_getidm(IntPtr f, byte[] data);
        [DllImport(DLL_NAME_FELICA)] private extern static void felica_getpmm(IntPtr f, byte[] data);
        [DllImport(DLL_NAME_FELICA)] private extern static int felica_read_without_encryption02(IntPtr f, int servicecode, int mode, byte addr, byte[] data);
        #endregion

        private IntPtr pasoriP = IntPtr.Zero;
        private IntPtr felicaP = IntPtr.Zero;

        public Felica()
        {
            pasoriP = pasori_open(null);
            if (pasoriP == IntPtr.Zero)
            {
                Debug.LogError($"{DLL_NAME_FELICA}を開けません");
                return;
            }
            if (pasori_init(pasoriP) != 0)
            {
                Debug.LogWarning("PaSoRiに接続できません");
                return;
            }
        }

        public void Dispose()
        {
            if (pasoriP != IntPtr.Zero)
            {
                pasori_close(pasoriP);
                pasoriP = IntPtr.Zero;
            }
        }

        ~Felica() => Dispose();

        public void Polling(int systemCode)
        {
            felica_free(felicaP);

            felicaP = (IntPtr)felica_polling(pasoriP, (ushort)systemCode, 0, 0);
            if (felicaP == IntPtr.Zero)
            {
                //Debug.LogWarning("Felicaが見つかりません");
                return;
            }
        }

        public byte[] IDm()
        {
            if (felicaP == IntPtr.Zero)
            {
                throw new Exception("実行前にポーリングを行ってください");
            }

            var buf = new byte[8];
            felica_getidm(felicaP, buf);
            return buf;
        }

        public byte[] PMm()
        {
            if (felicaP == IntPtr.Zero)
            {
                throw new Exception("実行前にポーリングを行ってください");
            }

            var buf = new byte[8];
            felica_getpmm(felicaP, buf);
            return buf;
        }

        public byte[] ReadWithoutEncryption(int serviceCode, int addr)
        {
            if (felicaP == IntPtr.Zero)
            {
                throw new Exception("実行前にポーリングを行ってください");
            }

            var data = new byte[16];
            var ret = felica_read_without_encryption02(felicaP, serviceCode, 0, (byte)addr, data);
            if (ret != 0)
            {
                return null;
            }
            return data;
        }
    }
}