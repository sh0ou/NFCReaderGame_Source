using TMPro;
using UnityEngine;

namespace Jubatus
{
    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private TextMeshProUGUI text;
        private int score_now = 0;

        private void Awake()
        {
            ResetScore();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void AddScore(int score)
        {
            score_now += score;
            text.text = $"{score_now.ToString("0,000,000")}";
            anim.SetTrigger("Add");
        }

        public void ResetScore()
        {
            score_now = 0;
            text.text = $"{score_now.ToString("0,000,000")}";
        }

        /// <summary> スコアをリザルトに送信 </summary>
        public void SendScore()
        {
            if (FindAnyObjectByType<ResultUI>().TryGetComponent<ResultUI>(out var result))
            {
                //-,---,---p
                var str = $"{score_now.ToString("0,000,000")}<size=32>p</size>";

                //スコアを送信
                result.SetResultInfo(ResultUI.ResultInfoType.Score, str);
            }
        }
    }
}