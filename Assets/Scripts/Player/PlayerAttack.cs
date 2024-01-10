using UnityEngine;

namespace FlashSexJam.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        private Rigidbody2D _rb;

        public float MaxX { set; private get; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _rb.velocity = Vector2.right * 10f;
            if (transform.position.x > MaxX) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
