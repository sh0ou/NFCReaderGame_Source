using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
/// <summary> キー入力を確認するためのクラス </summary>

namespace Jubatus
{
    public class KeyConfigChecker : MonoBehaviour
    {
        [SerializeField, Label("対象Action")] private InputActionReference actionRef;
        [SerializeField, Label("チェックランプ画像")] private UnityEngine.UI.Image checkImage;
        [SerializeField, Label("対象Scheme")] private string targetScheme;

        private void Start()
        {

        }

        private void OnEnable()
        {
            actionRef.action.performed += OnPerformed;
            actionRef.action.canceled += OnCanceled;
        }

        private void OnDisable()
        {
            actionRef.action.performed -= OnPerformed;
            actionRef.action.canceled -= OnCanceled;
        }

        private void OnPerformed(InputAction.CallbackContext context)
        {
            //対象のコントロールスキームである場合のみ実行
            if (CheckCurrentScheme(context))
            {
                checkImage.color = Color.green;
            }
        }

        private void OnCanceled(InputAction.CallbackContext context)
        {
            if (CheckCurrentScheme(context))
            {
                checkImage.color = Color.white;
            }
        }

        private bool CheckCurrentScheme(InputAction.CallbackContext context)
        {
            //現在のコントロールスキームを取得
            var device = context.control.device;
            var controlSchemes = context.action.actionMap.controlSchemes;
            var nowControlScheme = controlSchemes.ToList().Find(x => x.SupportsDevice(device));

            if (nowControlScheme != null)
            {
                var controlSchemeName = nowControlScheme.name;
                //Debug.Log($"現在のコントロールスキーム：{controlSchemeName}", this);

                return (controlSchemeName == targetScheme);
            }
            else return false;
        }
    }
}