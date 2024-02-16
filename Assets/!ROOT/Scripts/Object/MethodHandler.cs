using UnityEngine;
using UnityEngine.Events;

namespace Jubatus
{
    public class MethodHandler : MonoBehaviour
    {
        [SerializeField] UnityEvent onInvoke;

        public void InvokeMethod() => onInvoke.Invoke();

    }
}