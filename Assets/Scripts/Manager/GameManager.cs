using FlashSexJam.SO;
using Buttplug.Client;
using UnityEngine;

namespace FlashSexJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private Transform _spawnPoint;

        private float _spawnTimer;

        public float Speed { private set; get; }

        private ButtplugClient _client;

        private void Awake()
        {
            Instance = this;
            Speed = _info.MinSpeed;

            ResetSpawnTimer();
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime * Speed; // Timer depends of which speed we are going to

            if (_spawnTimer <= 0)
            {
                Instantiate(_info.SpawnableEnemies[Random.Range(0, _info.SpawnableEnemies.Length)], _spawnPoint.position + (Vector3.up * Random.Range(-2f, 2f)), Quaternion.identity);

                ResetSpawnTimer();
            }
        }

        public void ResetSpawnTimer()
        {
            _spawnTimer = Random.Range(_info.SpawnIntervalMin, _info.SpawnIntervalMax);
        }

        public void IncreaseSpeed(float value)
        {
            Speed = Mathf.Clamp(Speed + (value * _info.SpeedChangeMultiplier), _info.MinSpeed, _info.MaxSpeed);
        }
    }
}
