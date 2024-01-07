using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _rb.velocity = Vector3.left * GameManager.Instance.Speed;
        }
    }
}
