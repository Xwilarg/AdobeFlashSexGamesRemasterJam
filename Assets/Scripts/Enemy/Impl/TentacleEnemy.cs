using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.Enemy.Impl
{
    public class TentacleEnemy : EnemyController
    {

        [SerializeField]
        private float _speedOffset;


        private void Update()
        {
            _rb.velocity = Vector3.left * (GameManager.Instance.Speed + _speedOffset);
        }
    }
}
