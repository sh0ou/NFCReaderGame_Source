using System.Net.WebSockets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Jubatus
{
    public class CountTimer : MonoBehaviour
    {
        [SerializeField, Label("カウント有効")] private bool isTick = false;
        [SerializeField, Label("時間制限")] private float timeLimit;
        private float time_now_down, time_now_up;
        [SerializeField, Header("タイムアップ時EV")] private UnityEvent onTimeUp;

        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {

        }

        private void Update()
        {
            if (isTick)
            {
                //時間表示
                time_now_down -= Time.deltaTime;
                text.text = time_now_down < 10
                    ? $"{(int)time_now_down}<size=42>.{(time_now_down - Mathf.FloorToInt(time_now_down)).ToString("F2").Replace("0.", "")}</size>"
                    : $"{(int)time_now_down}";
                //タイムアップ時
                if (time_now_down <= 0)
                {
                    onTimeUp.Invoke();
                }
            }
            else
            {
                text.text = "0";
            }

            if (isTick) time_now_up += Time.deltaTime;
        }

        public void ResetTimer(bool isUp)
        {
            if (isUp)
            {
                time_now_up = timeLimit;
            }
            else
            {
                time_now_down = timeLimit;
            }
        }
        public void SwitchTick(bool flag) => isTick = flag;

        public void SendTime(bool isClear = false)
        {
            if (FindAnyObjectByType<ResultUI>().TryGetComponent<ResultUI>(out var result))
            {
                //Time: {分}:{秒}.{ミリ秒}}
                var str = isClear
                    ? $"Time: {((int)time_now_up / 60).ToString("D2")}" +
                    $":{((int)time_now_up % 60).ToString("D2")}" +
                    $"<size=24>.{(time_now_up - Mathf.FloorToInt(time_now_up)).ToString("F2").Replace("0.", "")}</size>"
                    : "Time: --:--<size=24>.--</size>";

                //結果画面に送信
                result.SetResultInfo(ResultUI.ResultInfoType.Time, str);
            }
        }
    }
}