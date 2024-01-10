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

            if (collision.CompareTag("Enemy") && Owner.gameObject.activeInHierarchy && !Owner.IsInvulnerable)
            {
                if (Owner.TryBreakCloth(Type))
                {
                    Owner.ToggleInvulnerabilityFrames();
                }
                else
                {
                    var prefab = collision.GetComponent<EnemyController>().GetHScene(Type);
                    if (prefab == null)
                    {
                        Debug.LogWarning($"Animation for {Type} was null");
                        return;
                    }
                    var go = Instantiate(prefab, Owner.transform.position, Quaternion.identity);
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
