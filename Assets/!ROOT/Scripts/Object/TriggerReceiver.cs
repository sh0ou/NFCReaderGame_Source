using UnityEngine;
using UnityEngine.Events;

namespace Jubatus
{
    public class TriggerReceiver : MonoBehaviour
    {
        [SerializeField, Header("判定対象のタグ")] private string[] targetTags;
        //[SerializeField, Header("判定対象のコライダー")] private Collider[] targetCollider;
        [SerializeField, Header("Enter時イベント")] private UnityEvent evTriggerEnter;
        [SerializeField, Header("Exit時イベント")] private UnityEvent evTriggerExit;

        protected virtual void OnTriggerEnter(Collider targetCol)
        {
            //タグ一致チェック
            foreach (var tag in targetTags)
            {
                if (targetCol.CompareTag(tag))
                {
                    //Debug.Log($"Trigger! {transform.root.name} >> {other.transform.root.name}", this);
                    CustomTriggerEnter(targetCol.gameObject);
                    evTriggerEnter.Invoke();
                    return;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //タグ一致チェック
            foreach (var tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    //Debug.Log($"Trigger! {transform.root.name} >> {other.transform.root.name}", this);
                    CustomTriggerExit(other.gameObject);
                    evTriggerExit.Invoke();
                    return;
                }
            }
        }

        protected virtual void CustomTriggerEnter(GameObject obj) { }
        protected virtual void CustomTriggerExit(GameObject obj) { }
    }
}