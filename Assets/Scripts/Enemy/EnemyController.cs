using FlashSexJam.Manager;
using FlashSexJam.Player;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hSceneHead, _hSceneUpperBody, _hSceneLowerBody;

        [SerializeField]
        private float _xSpeedOffset;

        public GameObject GetHScene(BodyPartType type)
        {
            return type switch
            {
                BodyPartType.Head => _hSceneHead,
                BodyPartType.UpperBody => _hSceneUpperBody,
                BodyPartType.LowerBody => _hSceneLowerBody,
                _ => null
            };
        }

        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            var (Min, Max) = SpawnRange;
            var yPos = Min == Max ? Min : Random.Range(Min, Max);
            transform.Translate(Vector2.up * yPos);
        }

        protected virtual void Update()
        {
            _rb.velocity = Vector3.left * (GameManager.Instance.Speed + _xSpeedOffset);
        }

        public abstract (float Min, float Max) SpawnRange { get; }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Boss"))
            {
                Destroy(gameObject);
            }
        }
    }
}
