using FlashSexJam.Enemy;
using UnityEngine;

namespace FlashSexJam.Player
{
    public class BodyPart : MonoBehaviour
    {
        public PlayerController Owner { set; private get; }

        public BodyPartType Type { set; private get; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy") && Owner.gameObject.activeInHierarchy)
            {
                if (!Owner.TryBreakCloth(Type))
                {
                    Owner.gameObject.SetActive(false);
                    Instantiate(collision.GetComponent<EnemyController>().HScene, Owner.transform.position, Quaternion.identity);
                }
            }
        }
    }

    public enum BodyPartType
    {
        Head,
        UpperBody,
        LowerBody
    }
}
