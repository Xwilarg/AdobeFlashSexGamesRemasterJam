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
            if (GameManager.Instance.DidGameEnd) return;

            if (collision.CompareTag("Enemy") && Owner.gameObject.activeInHierarchy)
            {
                if (!Owner.TryBreakCloth(Type))
                {
                    var go = Instantiate(collision.GetComponent<EnemyController>().HScene, Owner.transform.position, Quaternion.identity);
                    go.GetComponent<HScenePositionData>().BreakClothes(Owner);
                    Owner.HScene.PlayHScene(go);
                }
                Destroy(collision.gameObject);
            }
            else if (collision.CompareTag("Boss"))
            {
                GameManager.Instance.TriggerGameOver();
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
