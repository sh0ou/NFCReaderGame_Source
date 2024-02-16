using System.Collections;
using UnityEngine;

namespace Jubatus
{
    public sealed class DamageReceiver : TriggerReceiver
    {
        [SerializeField] private CharacterAction statusController;

        private float knockbackStrength = 5.0f;
        private float drag = 0.1f;

        public enum TriggerType
        {
            Hit,
            Damage
        }
        [SerializeField] private TriggerType triggerType;

        protected override void CustomTriggerEnter(GameObject target)
        {
            //Debug.Log("target:" + target.transform.root.name);
            statusController.Damage(target.transform.root.GetComponent<CharacterStatus>().atk);
            if (triggerType == TriggerType.Damage)
            {
                Knockback(target);
            }
        }

        private void Knockback(GameObject attacker)
        {
            var rb = transform.root.GetComponent<Rigidbody>();
            var dir = (transform.position - attacker.transform.position).normalized;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * knockbackStrength, ForceMode.Impulse);

            StartCoroutine(ApplyDrag(rb));
        }

        private IEnumerator ApplyDrag(Rigidbody rb)
        {
            while (rb.velocity.magnitude > 0.2f)
            {
                rb.velocity *= (1 - drag);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}