using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jubatus
{
    public class TagAnimation : MonoBehaviour
    {
        private Animator anim;
        [SerializeField] private string[] targetTags;
        [SerializeField] private string[] animNames;

        private void Awake()
        {
            anim = GetComponent<Animator>();

            //タグとアニメーションの数が一致しているか確認
            if (targetTags.Length != animNames.Length)
            {
                Debug.LogError("タグとアニメーションの数が一致しません", this);
                return;
            }

            SetAnim();
        }

        /// <summary>
        /// タグに応じてアニメーションを再生
        /// </summary>
        public void SetAnim()
        {
            for (var i = 0; i < targetTags.Length; i++)
            {
                if (gameObject.tag == targetTags[i])
                {
                    anim.Play(animNames[i]);
                }
            }
        }
    }
}