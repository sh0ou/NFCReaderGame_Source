using TMPro;
using UnityEngine;

namespace Jubatus
{
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private TextMeshProUGUI text_score, text_wave, text_time;
        private KeyToGauge keyToGauge;

        public void SwitchCanExit() => keyToGauge.isCanFill = !keyToGauge.isCanFill;

        public enum ResultInfoType { Score, Wave, Time };

        //private float titleGaugefill = 0;

        private void Awake()
        {
            keyToGauge = GetComponent<KeyToGauge>();
            keyToGauge.isCanFill = false;
        }

        private void OnEnable()
        {
            keyToGauge.onGaugeFilled += ExitResult;
        }

        private void OnDisable()
        {
            keyToGauge.onGaugeFilled -= ExitResult;
        }

        private void Update()
        {

        }

        public void ExitResult()
        {
            //タイトルへ
            anim.SetTrigger("Exit");
            keyToGauge.isCanFill = false;
        }

        /// <summary> スコア、ウェーブ、タイムを設定 </summary>
        /// <param name="type">設定項目</param>
        /// <param name="text">値</param>
        public void SetResultInfo(ResultInfoType type, string text)
        {
            switch (type)
            {
                case ResultInfoType.Score:
                    this.text_score.text = text;
                    break;
                case ResultInfoType.Wave:
                    this.text_wave.text = text;
                    break;
                case ResultInfoType.Time:
                    this.text_time.text = text;
                    break;
            }
        }
    }
}