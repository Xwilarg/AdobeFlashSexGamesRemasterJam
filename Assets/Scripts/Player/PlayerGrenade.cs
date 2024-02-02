using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Player
{
    public class PlayerGrenade : MonoBehaviour
    {
        protected Rigidbody2D _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            _rb.velocity = Vector3.right * GameManager.Instance.GetBossSpeed();
        }
    }
}
