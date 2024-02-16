using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogViewer : MonoBehaviour
{
    //ログ出力先Text
    [SerializeField] private TextMeshProUGUI _text;
    //スクロールバー
    [SerializeField] private ScrollRect _scrollRect;
    //無視する単語
    [SerializeField] private string[] _ignoreWords;

    [Label("自動スクロール")] public bool isAutoScroll = true;
    [Label("タイムスタンプ生成")] public bool isTimeStamp = true;

    private void Awake()
    {
        //AddInputListener();
        ResetLog();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowViewerWindow(!_scrollRect.gameObject.activeSelf);
        }
    }

    private void OnEnable()
    {
        //ログ出力イベントを登録
        Application.logMessageReceived += OnLogMessageReceived;

        //BaseInput.Enable();
    }

    private void OnDisable()
    {
        //ログ出力イベントを解除
        Application.logMessageReceived -= OnLogMessageReceived;

        //BaseInput.Disable();
    }

    private void OnLogMessageReceived(string logText, string stackTrace, LogType logType)
    {
        //無視する単語があれば表示しない
        foreach (var ignoreWord in _ignoreWords)
        {
            if (logText.Contains(ignoreWord)) return;
        }

        //ログの種類を大文字に変換
        var logType_upper = logType.ToString().ToUpper();
        //エラー属性に色を付ける
        var colorText = "#717375";
        switch (logType)
        {
            case LogType.Log:
                logType_upper = "LOG";
                colorText = "#717375";
                break;
            case LogType.Warning:
                logType_upper = "WRN";
                colorText = "#FFFF00";
                break;
            case LogType.Error:
                logType_upper = "ERR";
                colorText = "#FF0000";
                break;
            case LogType.Exception:
                logType_upper = "EXC";
                colorText = "#FF0000";
                break;
            case LogType.Assert:
                logType_upper = "AST";
                colorText = "#FF0000";
                break;
        }

        //タイムスタンプ用の文字列を作成
        var timeStamp = "";
        if (isTimeStamp) timeStamp = $"[{System.DateTime.Now.ToString("HH:mm:ss")}]";

        //ログを更新
        UpdateLogMessage($"{timeStamp}<color={colorText}>{logType_upper}</color>: {logText}");
    }

    /// <summary>ログを更新</summary>
    private void UpdateLogMessage(string logString)
    {
        _text.text += logString + System.Environment.NewLine;

        //スクロールバーを一番下に移動
        if (isAutoScroll) _scrollRect.velocity = new Vector2(0f, 10000f);
    }

    /// <summary>ログ出力先の表示を切り替える</summary>
    public void ShowViewerWindow(bool isActive)
    {
        _scrollRect.gameObject.SetActive(isActive);
    }

    /// <summary>ログをリセットする</summary>
    public void ResetLog()
    {
        _text.text = "";
    }
}
