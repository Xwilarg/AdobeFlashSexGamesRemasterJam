using FlashSexJam.Enemy;
using FlashSexJam.Manager;
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
                    GameManager.Instance.Speed = 0f;
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
