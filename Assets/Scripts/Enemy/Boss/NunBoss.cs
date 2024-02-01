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
        private GameObject _grenade;

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

            var isGrenade = Random.Range(0, 5) == 0;
            var prefab = isGrenade ? _grenade : _enemy;

            var go = GameManager.Instance.SpawnEnemy(prefab, PlayerId);
            if (!isGrenade)
            {
                go.GetComponent<EnemyController>().OverridesSpawnPos = false;
            }
            go.transform.position = _lowerSpawn.position;
        }
    }
}
