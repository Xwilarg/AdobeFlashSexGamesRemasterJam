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
            GameManager.Instance.HitEnemy(Owner.PlayerID);
            if (Owner.TryBreakCloth(Type))
            {
                Owner.ToggleInvulnerabilityFrames();
            }
            else
            {
                var controller = other.GetComponent<EnemyController>();
                var prefab = controller.GetHScene(Type);
                if (prefab == null)
                {
                    Debug.LogWarning($"Animation for {Type} was null");
                    return;
                }
                var go = Instantiate(prefab, Owner.transform.position, Quaternion.identity);
                go.GetComponent<HScenePositionData>().InitClothes(Owner);
                Owner.HScene.PlayHScene(go, controller.Name.GetHashCode());
            }
            Destroy(other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameManager.Instance.DidGameEnd(Owner.PlayerID)) return;

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
        }
    }

    public enum BodyPartType
    {
        Head,
        UpperBody,
        LowerBody
    }
}
