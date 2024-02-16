using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Jubatus
{
    /// <summary>
    /// ゲージに追従します
    /// </summary>
    public class FollowGauge : MonoBehaviour
    {
        private Image thisGauge;
        [SerializeField, Label("追従対象ゲージ")] private Image targetGauge;
        private void OnEnable()
        {
            thisGauge = GetComponent<Image>();
            DOTween.SetTweensCapacity(5000, 12500);
        }

        // Update is called once per frame
        private void Update()
        {
            if (thisGauge.fillAmount != targetGauge.fillAmount) { OnChange(); }
        }
        public void OnChange()
        {
            thisGauge.DOFillAmount(targetGauge.fillAmount, 0.5f).SetDelay(0.5f).SetLink(gameObject);
        }
    }
}