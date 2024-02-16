using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Jubatus
{
    public class KeyConfigRebinder : MonoBehaviour
    {
        [SerializeField, Label("リバインド対象Action")] private InputActionReference actionRef;
        [SerializeField, Label("リバインド対象Scheme")] private string scheme = "Keyboard";
        [SerializeField, Label("バインドパステキスト")] private TextMeshProUGUI bindPathText;
        [SerializeField, Label("リバインドターゲット表示テキスト")] private TextMeshProUGUI bindTargetText;
        [SerializeField, Label("リバインド受付画面Obj")] private GameObject maskObj;
        [SerializeField, Header("キャンセルキー")] private InputAction cancelKey;

        private InputAction action;
        private InputActionRebindingExtensions.RebindingOperation rebindOperation;

        private void Awake()
        {
            if (actionRef == null) return;
            if (cancelKey.bindings.Count != 1)
            {
                Debug.LogError("キャンセルキーのバインドがない、もしくは単体ではありません", this);
                enabled = false;
                return;
            }

            action = actionRef.action;

            RefreshBindPath();
        }

        private void OnDestroy()
        {
            CleanUpOperation();
        }

        public void StartRebinding()
        {
            if (action == null) return;

            var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(scheme));

            OnStartRebinding(bindingIndex);
        }

        /// <summary> リバインドを開始する </summary>
        public void OnStartRebinding(int bindingIndex)
        {
            //リバインド中の場合、強制キャンセル
            rebindOperation?.Cancel();

            action.Disable();

            if (maskObj != null) maskObj.SetActive(true);

            var bindings = action.bindings;

            bindTargetText.text = bindings.Count > 1
                ? $"現在のキー：{bindings[bindingIndex].name}"
                : "";

            //リバインド終了時に実行
            void OnFinished(bool hideMask = true)
            {
                CleanUpOperation();

                action.Enable();

                if (maskObj != null && hideMask) maskObj.SetActive(false);
            }

            //リバインド開始
            rebindOperation = action
                .PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .OnComplete(_ =>
                {
                    RefreshBindPath();

                    var nextBindingIndex = bindingIndex + 1;

                    //次のバインドが存在する場合、リバインドを続ける
                    if (nextBindingIndex <= bindings.Count - 1 &&
                        bindings[nextBindingIndex].isPartOfComposite)
                    {
                        OnFinished(false);
                        OnStartRebinding(nextBindingIndex);
                    }
                    else
                    {
                        OnFinished();
                    }
                })
                .OnCancel(_ => OnFinished())
                .OnMatchWaitForAnother(0.2f)
                .WithCancelingThrough(cancelKey.bindings[0].path)
                .Start();
        }

        /// <summary> キーバインドをリセットする </summary>
        public void ResetKeyBind()
        {
            action?.RemoveAllBindingOverrides();
            RefreshBindPath();
        }

        /// <summary> バインドパスのテキストを更新する </summary>
        public void RefreshBindPath()
        {
            if (action == null || bindPathText == null) return;

            bindPathText.text = action.GetBindingDisplayString(group: scheme);
        }

        /// <summary> リバインドオペレーションを破棄 </summary>
        private void CleanUpOperation()
        {
            rebindOperation?.Dispose();
            rebindOperation = null;
        }
    }
}