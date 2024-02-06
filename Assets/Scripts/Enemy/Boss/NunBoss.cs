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
        private Transform _lowerSpawn, _midSpawn;

        private Animator _anim;

        public int PlayerId { set; private get; }

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        public IEnumerator Attack()
        {
            Vector3 spawnPos;
            var rand = Random.Range(0, 2);
            if (rand == 0)
            {
                _anim.SetTrigger("LowAttack");
                spawnPos = _lowerSpawn.position;
            }
            else
            {
                _anim.SetTrigger("MidAttack");
                spawnPos = _midSpawn.position;
            }

            yield return new WaitForSeconds(.68f);

            var isGrenade = Random.Range(0, 5) == 0;
            var prefab = isGrenade ? _grenade : _enemy;

            var go = GameManager.Instance.SpawnEnemy(prefab, PlayerId);
            if (!isGrenade)
            {
                go.GetComponent<EnemyController>().OverridesSpawnPos = false;
            }
            go.transform.position = spawnPos;
        }
    }
}
