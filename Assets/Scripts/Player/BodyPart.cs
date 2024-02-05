using FlashSexJam.Enemy;
using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Player
{
    public class BodyPart : MonoBehaviour
    {
        public PlayerController Owner { set; private get; }

        public BodyPartType Type { set; private get; }

        private void StartHScene(GameObject prefab, EnemyController controller)
        {
            var go = Instantiate(prefab, Owner.transform.position, Quaternion.identity);
            go.GetComponent<HScenePositionData>().InitClothes(Owner);
            Owner.HScene.PlayHScene(go, controller.Name.GetHashCode());
        }
        private void EnemyCollide(GameObject other)
        {
            GameManager.Instance.HitEnemy(Owner.PlayerID);
            if (Owner.TryBreakCloth(Type))
            {
                Owner.ToggleClothDamage(Type);
            }
            else
            {
                var controller = other.GetComponent<EnemyController>();
                var prefab = controller.GetHScene(Type);
                if (prefab == null)
                {
                    if (!Owner.IsTopBodyBroken)
                    {
                        Owner.TryBreakCloth(BodyPartType.UpperBody);
                        Owner.ToggleClothDamage(Type);
                    }
                    else if (!Owner.IsLowerBodyBroken)
                    {
                        Owner.TryBreakCloth(BodyPartType.LowerBody);
                        Owner.ToggleClothDamage(Type);
                    }
                    else
                    {
                        var parts = new[] { BodyPartType.Head, BodyPartType.UpperBody, BodyPartType.LowerBody };
                        foreach (var p in parts)
                        {
                            prefab = controller.GetHScene(p);
                            if (prefab != null)
                            {
                                break;
                            }
                        }
                        if (prefab == null)
                        {
                            Debug.LogError($"No H scene available");
                        }
                        else
                        {
                            StartHScene(prefab, controller);
                        }
                    }
                }
                else
                {
                    StartHScene(prefab, controller);
                }
            }
            Destroy(other);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameManager.Instance.DidGameEnd(Owner.PlayerID)) return;

            if (Owner.gameObject.activeInHierarchy && !Owner.IsInvulnerable)
            {
                if (collision.CompareTag("Grenade"))
                {
                    Owner.GetAttackPowerup();
                    Destroy(collision.gameObject);
                }
                else if (collision.CompareTag("Enemy"))
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
