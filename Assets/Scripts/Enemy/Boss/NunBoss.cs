using FlashSexJam.Manager;
using System.Collections;
using UnityEngine;

namespace FlashSexJam.Enemy.Boss
{
    public class NunBoss : MonoBehaviour
    {
        [SerializeField]
        private GameObject _enemy;

        [SerializeField]
        private Transform _lowerSpawn;

        private Animator _anim;

        public int PlayerId { set; private get; }

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public IEnumerator Attack()
        {
            _anim.SetTrigger("LowAttack");

            yield return new WaitForSeconds(.68f);

            var go = GameManager.Instance.SpawnEnemy(_enemy, PlayerId);
            go.GetComponent<EnemyController>().OverridesSpawnPos = false;
            go.transform.position = _lowerSpawn.position;
        }
    }
}
