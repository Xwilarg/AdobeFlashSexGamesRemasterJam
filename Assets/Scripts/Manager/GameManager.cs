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

        [SerializeField]
        private RectTransform _progressPlayerBar, _progressBossBar;

        private float _spawnTimer;

        private float _progress, _progressBoss;

        public float Speed { private set; get; }

        private ButtplugClient _client;

        private void Awake()
        {
            Instance = this;
            Speed = _info.MinSpeed;
            _progressBoss = -_info.BossNegativeOffset;

            ResetSpawnTimer();
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime * Speed; // Timer depends of which speed we are going to
            _progress += Time.deltaTime * Speed;

            _progressPlayerBar.localScale = new(_progress / _info.DestinationDistance, 1f, 1f);
            _progressBossBar.localScale = new(_progressBoss / _info.DestinationDistance, 1f, 1f);

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

        public void StopSpeed()
        {
            Speed = 0;
        }

        public void ResetSpeed()
        {
            Speed = _info.MinSpeed;
        }
    }
}
