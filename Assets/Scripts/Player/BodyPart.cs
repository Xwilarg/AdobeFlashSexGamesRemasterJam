using FlashSexJam.Enemy;
using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Player
{
    public class BodyPart : MonoBehaviour
    {
        public PlayerController Owner { set; private get; }

        public BodyPartType Type { set; private get; }

        private void EnemyCollide(GameObject other)
        {
            if (Owner.TryBreakCloth(Type))
            {
                Owner.ToggleInvulnerabilityFrames();
            }
            else
            {
                var prefab = other.GetComponent<EnemyController>().GetHScene(Type);
                if (prefab == null)
                {
                    Debug.LogWarning($"Animation for {Type} was null");
                    return;
                }
                var go = Instantiate(prefab, Owner.transform.position, Quaternion.identity);
                go.GetComponent<HScenePositionData>().BreakClothes(Owner);
                Owner.HScene.PlayHScene(go);
            }
            Destroy(other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameManager.Instance.DidGameEnd) return;

            if (Owner.gameObject.activeInHierarchy && !Owner.IsInvulnerable)
            {
                if (collision.CompareTag("Enemy"))
                {
                    EnemyCollide(collision.gameObject);
                }
                else if (collision.CompareTag("EnemyChildCollider"))
                {
                    EnemyCollide(collision.transform.parent.gameObject);
                }
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
