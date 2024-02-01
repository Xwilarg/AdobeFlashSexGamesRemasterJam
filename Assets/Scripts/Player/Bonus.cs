using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Player
{
    public class Bonus : MonoBehaviour
    {
        [SerializeField]
        private float _xSpeedOffset;

        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            _rb.velocity = Vector3.left * (GameManager.Instance.GetBossSpeed() + _xSpeedOffset); // Speed of the same for all player in boss stage

            if (transform.position.x < -20f)
            {
                Destroy(gameObject);
            }
        }
    }
}
