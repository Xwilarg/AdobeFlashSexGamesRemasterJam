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
                    var go = Instantiate(collision.GetComponent<EnemyController>().HScene, Owner.transform.position, Quaternion.identity);
                    go.GetComponent<HScenePositionData>().BreakClothes(Owner);
                    GameManager.Instance.Speed = 0f;
                    Owner.gameObject.SetActive(false);
                }
                Destroy(collision.gameObject);
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
