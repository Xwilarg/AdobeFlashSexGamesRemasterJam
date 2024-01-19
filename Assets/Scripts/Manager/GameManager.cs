using FlashSexJam.SO;
using Buttplug.Client;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FlashSexJam.Achievement;
using FlashSexJam.Player;
using System.Collections.Generic;
using System.Linq;

namespace FlashSexJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private RectTransform _progressPlayerBar, _progressBossBar, _goalProgressBar, _startProgressBar;

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

        private readonly Dictionary<int, PlayerData> _players = new();

        private ButtplugClient _client;

        private readonly List<int> _enemyHScenes = new();

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

            foreach (var player in _players.Values)
            {
                player.WallOfTentacles.position = new(_progressBoss - _progress, player.WallOfTentacles.position.y);
            }

            _progressPlayerBar.transform.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, _progress / _info.DestinationDistance), _progressPlayerBar.transform.position.y, _progressPlayerBar.transform.position.z);
            _progressBossBar.transform.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, _progressBoss / _info.DestinationDistance), _progressBossBar.transform.position.y, _progressBossBar.transform.position.z);

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

                // We base achievements on the first player
                var hasClothes = _players.Values.All(x => x.PC.IsFullClothed);
                var hasPower = _players.Values.All(x => x.PC.IsFullyPowered);
                var hasNoHScenes = _players.Values.All(x => x.PC.GotHScene);

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
                foreach (var player in _players.Values)
                {
                    Instantiate(_info.SpawnableEnemies[Random.Range(0, _info.SpawnableEnemies.Length)], player.Spawner.position, Quaternion.identity);
                }

                ResetSpawnTimer();
            }

            if (DidGameEnd && _gameOverTimer < _gameOverTimerRef)
            {
                _gameOverTimer += Time.deltaTime;

                var c = _gameOverBackground.color;
                _gameOverBackground.color = new(c.r, c.g, c.b, Mathf.Clamp01(_gameOverTimer / _gameOverTimerRef));
            }
        }

        public void RegisterPlayer(Transform enemySpawn, Transform tentacles, PlayerController pc, Camera cam)
        {
            _players.Add(pc.gameObject.GetInstanceID(), new()
            {
                Cam = cam,
                PC = pc,
                Spawner = tentacles,
                WallOfTentacles = tentacles
            });

            UpdateCameras();
        }

        public void UnregisterPlayer(PlayerController pc)
        {
            _players.Remove(pc.gameObject.GetInstanceID());

            UpdateCameras();
        }

        private void UpdateCameras()
        {
            var max = 1f / _players.Count;
            for (int i = 0; i < _players.Count; i++)
            {
                _players.Values.ElementAt(i).Cam.rect = new(0f, max * i, 1f, max);
            }
        }

        public void HitEnemy()
        {
            _maxSpeedTimer = 0f;
        }

        public void PlayHScene(int id)
        {
            Speed = 0;
            if (!_enemyHScenes.Contains(id))
            {
                _enemyHScenes.Add(id);
                if (_enemyHScenes.Count == _info.SpawnableEnemies.Length)
                {
                    AchievementManager.Instance.Unlock(AchievementID.AllHScenes);
                }
            }
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

        public void ResetSpeed()
        {
            Speed = _info.MinSpeed;
        }

        public class PlayerData
        {
            public PlayerController PC;
            public Transform Spawner;
            public Transform WallOfTentacles;
            public Camera Cam;
        }
    }
}
