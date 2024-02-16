using UnityEngine;

namespace Jubatus
{
    /// <summary>
    /// フィールド上のアイテムを拾うためのクラス
    /// </summary>
    public sealed class ItemReceiver : TriggerReceiver
    {
        [SerializeField] private TokenStorage tokenStorage;

        protected override void CustomTriggerEnter(GameObject obj)
        {
            switch (obj.tag)
            {
                case "Token_R":
                    if (tokenStorage.AddToken(TokenStorage.TokenCat.Red)) Destroy(obj);
                    break;
                case "Token_G":
                    if (tokenStorage.AddToken(TokenStorage.TokenCat.Green)) Destroy(obj);
                    break;
                case "Token_B":
                    if (tokenStorage.AddToken(TokenStorage.TokenCat.Blue)) Destroy(obj);
                    Destroy(obj);
                    break;
                case "Token_X":
                    if (tokenStorage.AddToken(TokenStorage.TokenCat.X)) Destroy(obj);
                    break;
                default:
                    break;
            }
        }
    }
}