using FlashSexJam.Player;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hSceneHead, _hSceneUpperBody, _hSceneLowerBody;
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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Boss"))
            {
                Destroy(gameObject);
            }
        }
    }
}
