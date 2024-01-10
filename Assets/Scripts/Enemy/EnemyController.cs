using FlashSexJam.Manager;
using FlashSexJam.Player;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public class EnemyController : MonoBehaviour
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

        [SerializeField]
        private float _speedOffset;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _rb.velocity = Vector3.left * (GameManager.Instance.Speed + _speedOffset);
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
