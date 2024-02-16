using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Jubatus
{
    public class KeyToGauge : MonoBehaviour
    {
        [SerializeField] private Image gaugeUI;
        [SerializeField] private bool isReverse = false;
        public bool isCanFill = false;
        public void SetCanFill(bool value) => isCanFill = value;

        public delegate void OnGaugeFilled();
        public OnGaugeFilled onGaugeFilled;

        [SerializeField] private UnityEvent ev_Filled;

        private void Start()
        {
            gaugeUI.fillAmount = isReverse ? 1 : 0;
        }

        private void Update()
        {
            if (isCanFill) CheckKey();
        }

        private void CheckKey()
        {
            //いずれかのキーが長押しされたら
            if (Input.anyKey)
            {
                if (!isReverse)
                {
                    //ゲージが溜まる
                    gaugeUI.fillAmount += Time.deltaTime;
                    if (gaugeUI.fillAmount >= 1)
                    {
                        onGaugeFilled?.Invoke();
                        ev_Filled?.Invoke();
                    }
                }
                else
                {
                    //ゲージが減る
                    gaugeUI.fillAmount -= Time.deltaTime;
                    if (gaugeUI.fillAmount <= 0)
                    {
                        onGaugeFilled?.Invoke();
                        ev_Filled?.Invoke();
                    }
                }
            }
            else
            {
                if (!isReverse)
                {
                    gaugeUI.fillAmount -= Time.deltaTime * 0.5f;
                }
                else
                {
                    gaugeUI.fillAmount += Time.deltaTime * 0.5f;
                }
                Mathf.Clamp(gaugeUI.fillAmount, 0, 1);
            }
        }
    }
}