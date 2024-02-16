using System;
using UnityEngine;

public class DebugLogModifier : MonoBehaviour
{
    private ILogHandler _defaultLogHandler;
    private ILogHandler _logHandler;

    private void Awake()
    {
        //ログ出力先を変更
        _defaultLogHandler = Debug.unityLogger.logHandler;
        _logHandler = new DebugLogHandler(_defaultLogHandler);
        Debug.unityLogger.logHandler = _logHandler;
        Debug.Log("Active", this);
    }

    private void OnDestroy()
    {
        //ログ出力先を元に戻す
        if (_defaultLogHandler != null)
            Debug.unityLogger.logHandler = _defaultLogHandler;

        //デバッグビルド、もしくはエディタ上のみログを出力
        Debug.unityLogger.logEnabled = Debug.isDebugBuild || Application.isEditor;
    }
}

public class DebugLogHandler : ILogHandler
{
    private readonly ILogHandler _logHandler;

    public DebugLogHandler(ILogHandler logHandler) => _logHandler = logHandler;

    public void LogException(Exception exception, UnityEngine.Object context) => _logHandler.LogException(exception, context);

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        //ログ呼び出し元のクラス名を取得
        var className = "";
        if (context is not null)
            className = $"[<color=#7fffd4>{context.GetType().Name}</color>] ";

        //ログを出力
        _logHandler.LogFormat(logType, context, $"{className}{format}", args);
    }
}
