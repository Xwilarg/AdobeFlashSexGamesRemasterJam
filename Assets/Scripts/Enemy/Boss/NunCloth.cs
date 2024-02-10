using UnityEngine;

namespace FlashSexJam.Enemy.Boss
{
    public class NunCloth : MonoBehaviour
    {
        public NunBoss Owner { set; private get; }

        [SerializeField]
        private GameObject[] _clothParts;

        public bool IsBroken { private set; get; }

        public void Break()
        {
            if (IsBroken) return;

            foreach (var go in _clothParts)
            {
                go.SetActive(false);
            }
            IsBroken = true;
            Owner.TakeDamage();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Break();
            Destroy(collision.gameObject);
        }
    }
}
