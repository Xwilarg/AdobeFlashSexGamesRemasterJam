using FlashSexJam.SO;
using Buttplug.Client;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FlashSexJam.Achievement;
using System.Collections;
using FlashSexJam.Player;

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

        [SerializeField]
        private Transform _wallOfTentacles;

        [SerializeField]
        private GameObject _gameOverContainer;

        [SerializeField]
        private Image _gameOverBackground;

        [SerializeField]
        private GameObject _victoryContainer;

        private float _gameOverTimer;
        private const float _gameOverTimerRef = 3f;

        private float _spawnTimer;

        private float _progress, _progressBoss;

        private float _maxSpeedTimer;
        private float _maxSpeedTimerRef = 3f;

        public float Speed { private set; get; }

        public bool DidGameEnd { private set; get; }

        public PlayerController Player { set; private get; }

        private ButtplugClient _client;

        private void Awake()
        {
            Instance = this;

            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            Speed = _info.MinSpeed;
            _progressBoss = -_info.BossNegativeOffset;

            ResetSpawnTimer();
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime * Speed; // Timer depends of which speed we are going to
            _progress += Time.deltaTime * Speed;
            _progressBoss += Time.deltaTime * _info.BossSpeed;

            _wallOfTentacles.position = new(_progressBoss - _progress, _wallOfTentacles.position.y);

            _progressPlayerBar.localScale = new(_progress / _info.DestinationDistance, 1f, 1f);
            _progressBossBar.localScale = new(_progressBoss / _info.DestinationDistance, 1f, 1f);

            if (!DidGameEnd)
            {
                if (Speed == _info.MaxSpeed)
                {
                    _maxSpeedTimer += Time.deltaTime;
                    if (_maxSpeedTimer >= _maxSpeedTimerRef)
                    {
                        _maxSpeedTimer = 0f;
                        AchievementManager.Instance.Unlock(AchievementID.MaxSpeed);
                    }
                }
                else
                {
                    _maxSpeedTimer = 0f;
                }
            }

            if (_progress >= _info.DestinationDistance)
            {
                DidGameEnd = true;

                var hasClothes = Player.IsFullClothed;
                var hasPower = Player.IsFullyPowered;
                var hasNoHScenes = Player.GotHScene;

                AchievementManager.Instance.Unlock(AchievementID.Victory);
                if (hasNoHScenes) AchievementManager.Instance.Unlock(AchievementID.VictoryNoHScene);
                if (hasClothes) AchievementManager.Instance.Unlock(AchievementID.VictoryNoClothDamage);
                if (hasPower) AchievementManager.Instance.Unlock(AchievementID.VictoryFullPower);
                if (hasNoHScenes && hasClothes && hasPower) AchievementManager.Instance.Unlock(AchievementID.VictoryPerfect);

                _victoryContainer.SetActive(true);
                return;
            }

            if (_spawnTimer <= 0)
            {
                Instantiate(_info.SpawnableEnemies[Random.Range(0, _info.SpawnableEnemies.Length)], _spawnPoint.position, Quaternion.identity);

                ResetSpawnTimer();
            }

            if (DidGameEnd && _gameOverTimer < _gameOverTimerRef)
            {
                _gameOverTimer += Time.deltaTime;

                var c = _gameOverBackground.color;
                _gameOverBackground.color = new(c.r, c.g, c.b, Mathf.Clamp01(_gameOverTimer / _gameOverTimerRef));
            }
        }

        public void HitEnemy()
        {
            _maxSpeedTimer = 0f;
        }

        public void TriggerGameOver()
        {
            if (DidGameEnd) return; // Just in case

            DidGameEnd = true;
            _gameOverContainer.SetActive(true);
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
