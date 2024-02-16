using CI.QuickSave;
using System.Collections.Generic;
using UnityEngine;

namespace Jubatus
{
    public class UserDataRDWR : MonoBehaviour
    {
        public ConfigData configData;
        public UserData userData;
        private const string PASSKEY = "qauX3P_h2P2Yl.BT";

        public enum UserDataType
        {
            Config,
            User,
        }

        public void Save(UserDataType type)
        {
            QuickSaveWriter writer;
            switch (type)
            {
                case UserDataType.Config:
                    writer = QuickSaveWriter.Create("Config");
                    writer.Write("KeyConfigs", configData.keyConfig)
                        .Write("BGMVolume", configData.bgmVoume)
                        .Write("SEVolume", configData.seVolume)
                        .Commit();
                    break;
                case UserDataType.User:
                    writer = QuickSaveWriter.Create("UserData", new QuickSaveSettings()
                    {
                        SecurityMode = SecurityMode.Aes,
                        Password = PASSKEY,
                        CompressionMode = CompressionMode.Gzip
                    });
                    writer.Write("HighScore", userData.highScore)
                        .Commit();
                    break;
            }
        }

        public void Load(UserDataType type)
        {
            QuickSaveReader reader;
            switch (type)
            {
                case UserDataType.Config:
                    reader = QuickSaveReader.Create("Config");
                    reader.Read<Dictionary<string, string>>("KeyConfigs", r => configData.keyConfig = r)
                        .Read<int>("BGMVolume", r => configData.bgmVoume = r)
                        .Read<int>("SEVolume", r => configData.seVolume = r);
                    break;
                case UserDataType.User:
                    reader = QuickSaveReader.Create("UserData", new QuickSaveSettings()
                    {
                        SecurityMode = SecurityMode.Aes,
                        Password = PASSKEY,
                        CompressionMode = CompressionMode.Gzip
                    });
                    reader.Read<int>("HighScore", r => userData.highScore = r);
                    break;
            }
        }
    }

    [System.Serializable]
    public class ConfigData
    {
        public Dictionary<string, string> keyConfig;
        public int bgmVoume;
        public int seVolume;
    }

    [System.Serializable]
    public class UserData
    {
        public int highScore;
    }
}