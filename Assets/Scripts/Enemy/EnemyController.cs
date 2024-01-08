using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hScene;
        public GameObject HScene => _hScene;

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
    }
}
