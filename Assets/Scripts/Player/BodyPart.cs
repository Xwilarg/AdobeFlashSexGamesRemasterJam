using UnityEngine;

namespace FlashSexJam.Player
{
    public class BodyPart : MonoBehaviour
    {
        public PlayerController Owner { set; private get; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("hi");
            }
        }
    }
}
