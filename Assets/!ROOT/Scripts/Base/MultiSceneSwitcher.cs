using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jubatus
{
    /// <summary> 複数シーンを追加・切り替えするクラス </summary>
    public class MultiSceneSwitcher : MonoBehaviour
    {
        [SerializeField, Label("基本シーン？")] private bool isBaseScene = false;
        [SerializeField, Label("デバッグ用シーン名")] private string scn_debugWindow = "DebugWindow";
        [SerializeField, Label("シーン追加する？")] private bool isAddScene = true;
        [SerializeField, Header("追加シーン名")] private string[] addSceneNames;

        private void Awake()
        {
            if (Debug.isDebugBuild && isBaseScene)
            {
                AddScene(scn_debugWindow);
            }
        }

        private void Start()
        {
            if (isBaseScene) Debug.Log("Active", this);
            if (isAddScene && addSceneNames.Length > 0)
            {
                foreach (var sceneName in addSceneNames)
                {
                    AddScene(sceneName);
                }
            }
        }

        public void AddScene(string sceneName) => StartCoroutine(OnAddScene(sceneName, true));
        public void LoadScene(string sceneName) => StartCoroutine(OnAddScene(sceneName, false));

        private IEnumerator OnAddScene(string sceneName, bool isAdditive = false)
        {
            var mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            var scene = SceneManager.LoadSceneAsync(sceneName, mode);

            if (scene is null)
            {
                Debug.Log("シーンが見つかりませんでした。", this);
                yield break;
            }

            scene.allowSceneActivation = false;

            //シーンの読み込みが完了するまで待機
            while (!scene.isDone)
            {
                //シーンの読み込みが完了したら
                if (scene.progress >= 0.9f)
                {
                    //シーンをアクティブにする
                    scene.allowSceneActivation = true;
                }
                yield return null;
            }

            yield return scene;
        }

        public void RemoveScene(string sceneName) => StartCoroutine(OnRemoveScene(sceneName));
        public void RemoveThisScene() => StartCoroutine(OnRemoveScene(gameObject.scene.name));

        private IEnumerator OnRemoveScene(string sceneName)
        {
            var scene = SceneManager.UnloadSceneAsync(sceneName);
            if (scene == null)
            {
                Debug.Log("シーンが見つかりませんでした。", this);
                yield break;
            }

            yield return scene;
        }
    }
}