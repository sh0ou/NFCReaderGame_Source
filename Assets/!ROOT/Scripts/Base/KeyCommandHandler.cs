using UnityEngine;
using UnityEngine.Events;

public class KeyCommandHandler : MonoBehaviour
{
    [SerializeField] private KeyCode[] targetKeyCodes;
    [SerializeField] private UnityEvent ev_OnCommanded;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            //指定したキーが同時に押されているか
            foreach (KeyCode keyCode in targetKeyCodes)
            {
                if (!Input.GetKey(keyCode))
                {
                    return;
                }
            }
            ev_OnCommanded?.Invoke();
        }
    }
}
