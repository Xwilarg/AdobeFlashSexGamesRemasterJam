using System.Collections;
using UnityEngine;

namespace FlashSexJam.Enemy.Impl
{
    public class LampEnemy : EnemyController
    {
        private Animator _lampAnim;

        public override (float Min, float Max) SpawnRange => (-4f, -4f);

        protected override void Awake()
        {
            base.Awake();

            _lampAnim = GetComponentInChildren<Animator>();
            StartCoroutine(DoPatern());
        }

        private IEnumerator DoPatern()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                _lampAnim.SetBool("IsHigh", true);
                yield return new WaitForSeconds(2f);
                _lampAnim.SetBool("IsHigh", false);
            }
        }
    }
}
