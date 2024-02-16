using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jubatus
{
    public class HPGaugeUI : MonoBehaviour
    {
        [SerializeField] private CharacterStatus status;
        [SerializeField] private Image gauge_now;

        private void Update()
        {
            //ゲージ更新
            gauge_now.fillAmount = (float)status.hp / status.maxHp;
        }
    }
}