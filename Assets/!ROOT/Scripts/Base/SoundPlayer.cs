using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jubatus
{
    public class SoundPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] clips;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayBGM(int id)
        {
            audioSource.clip = clips[id];
            audioSource.Play();
        }

        public void PlaySE(int id)
        {
            if (id is 8 or 9)
            {
                var walk = Random.Range(8, 9);
                audioSource.PlayOneShot(clips[walk]);
            }
            else
            {
                audioSource.PlayOneShot(clips[id]);
            }
        }
    }
}