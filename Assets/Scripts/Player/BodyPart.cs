using UnityEngine;

namespace FlashSexJam.Player
{
    public class BodyPart : MonoBehaviour
    {
        public PlayerController Owner { set; private get; }

        [SerializeField]
        private GameObject _clothPart;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                if (_clothPart != null)
                {
                    Destroy(_clothPart);
                    _clothPart = null;
                }
                else
                {
                    Debug.Log("ohno");
                }
            }
        }
    }
}
