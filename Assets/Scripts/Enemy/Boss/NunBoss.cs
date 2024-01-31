using UnityEngine;

namespace FlashSexJam.Enemy.Boss
{
    public class NunBoss : MonoBehaviour
    {
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public void Attack()
        {
            _anim.SetTrigger("LowAttack");
        }
    }
}
