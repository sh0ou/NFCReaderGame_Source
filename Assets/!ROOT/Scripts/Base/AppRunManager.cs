using Kogane;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jubatus
{
    public class AppRunManager : MonoBehaviour
    {
        //[SerializeField, Label("Baseシーン？")] private bool isBaseScene = false;

        public void Restart()
        {
            ApplicationRestarter.Restart();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}