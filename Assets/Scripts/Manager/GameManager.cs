using FlashSexJam.SO;
using Buttplug.Client;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FlashSexJam.Achievement;
using FlashSexJam.Player;
using System.Collections.Generic;
using System.Linq;
using FlashSexJam.Enemy;
using UnityEngine.InputSystem;

namespace FlashSexJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private Transform _bossBar;

        [SerializeField]
        private RectTransform _progressBossBar, _goalProgressBar, _startProgressBar;

        [SerializeField]
        private GameObject _gameOverContainer;

        [SerializeField]
        private Image _gameOverBackground;

        [SerializeField]
        private GameObject _victoryContainer;

        [SerializeField]
        private GameObject _playerUIProgPrefab;

        [SerializeField]
        private PlayerInputManager _inputManager;

        [SerializeField]
        private GameObject _playerContainerPrefab;

        [SerializeField]
        private GameObject _nextLevel;

        private int _levelIndex;

        public LevelInfo LevelInfo => _info.Levels[_levelIndex];

        private float _gameOverTimer;
        private const float _gameOverTimerRef = 3f;

        private float _progressBoss;

        private float _maxSpeedTimer;
        private float _maxSpeedTimerRef = 3f;

        public bool DidGameEnd { private set; get; }

        private readonly Dictionary<int, PlayerData> _players = new();

        private ButtplugClient _client;

        private readonly List<int> _enemyHScenes = new();

        private void Awake()
        {
            Instance = this;

            if (GlobalData.PlayerCount == 1) // Only one player, we disable the ability for anyone else to join and spawn the player
            {
                _inputManager.enabled = false;
                Instantiate(_playerContainerPrefab);
            }

            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            _progressBoss = -_info.BossNegativeOffset;
        }

        private void Update()
        {
            if (!_players.Any()) return;

            _progressBoss += Time.deltaTime * _info.BossSpeed;
            _progressBossBar.transform.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, _progressBoss / _info.DestinationDistance), _progressBossBar.transform.position.y, _progressBossBar.transform.position.z);

            foreach (var keyValue in _players)
            {
                var id = keyValue.Key;
                var player = keyValue.Value;

                player.SpawnTimer -= Time.deltaTime * player.Speed; // Timer depends of which speed we are going to
                player.Progress += Time.deltaTime * player.Speed;

                player.WallOfTentacles.position = new(_progressBoss - player.Progress, player.WallOfTentacles.position.y);

                player.UIProg.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, player.Progress / _info.DestinationDistance), player.UIProg.position.y, player.UIProg.position.z);

                if (!DidGameEnd)
                {
                    if (_players.First().Value.Speed == _info.MaxSpeed)
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

                if (player.Progress >= _info.DestinationDistance)
                {
                    DidGameEnd = true;

                    var targetPlayer = _players.First().Value;

                    // We base achievements on the first player
                    var hasClothes = targetPlayer.PC.IsFullClothed;
                    var hasPower = targetPlayer.PC.IsFullyPowered;
                    var hasNoHScenes = targetPlayer.PC.GotHScene;

                    AchievementManager.Instance.Unlock(AchievementID.Victory);
                    if (hasNoHScenes) AchievementManager.Instance.Unlock(AchievementID.VictoryNoHScene);
                    if (hasClothes) AchievementManager.Instance.Unlock(AchievementID.VictoryNoClothDamage);
                    if (hasPower) AchievementManager.Instance.Unlock(AchievementID.VictoryFullPower);
                    if (hasNoHScenes && hasClothes && hasPower) AchievementManager.Instance.Unlock(AchievementID.VictoryPerfect);

                    if (_levelIndex == _info.Levels.Length - 1)
                    {
                        _nextLevel.SetActive(false);
                    }
                    _victoryContainer.SetActive(true);
                    return;
                }

                if (player.SpawnTimer <= 0)
                {
                        var go = Instantiate(LevelInfo.SpawnableEnemies[Random.Range(0, LevelInfo.SpawnableEnemies.Length)], player.Spawner.position, Quaternion.identity);
                        go.GetComponent<EnemyController>().PlayerID = id;

                    ResetSpawnTimer(player);
                }
            }

            if (DidGameEnd && _gameOverTimer < _gameOverTimerRef)
            {
                _gameOverTimer += Time.deltaTime;

                var c = _gameOverBackground.color;
                _gameOverBackground.color = new(c.r, c.g, c.b, Mathf.Clamp01(_gameOverTimer / _gameOverTimerRef));
            }
        }

        public void NextLevel()
        {
            _levelIndex++;
            ResetGame();
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ResetGame()
        {
            _victoryContainer.SetActive(false);
            _progressBoss = -_info.BossNegativeOffset;

            foreach (var player in  _players.Values)
            {
                player.PC.ResetPlayer();

                player.Speed = _info.MinSpeed;
                player.Progress = 0f;
                player.SpawnTimer = 0f;
            }
        }

        public void RegisterPlayer(Transform enemySpawn, Transform tentacles, PlayerController pc, Camera cam)
        {
            pc.Color = _players.Any() ? new(0f, 0.1529f, 0.4509f) : new(0.4509f, 0.0235f, 0f);
            var data = new PlayerData()
            {
                Cam = cam,
                PC = pc,
                Spawner = enemySpawn,
                WallOfTentacles = tentacles,
                UIProg = Instantiate(_playerUIProgPrefab, _bossBar).transform,
                Speed = _info.MinSpeed,
                Progress = 0f,
                SpawnTimer = 0f
            };
            _players.Add(pc.gameObject.GetInstanceID(), data);
            ResetSpawnTimer(data);

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

        public void PlayHScene(int playerId, int id)
        {
            var player = _players[playerId];

            player.Speed = 0;
            if (!_enemyHScenes.Contains(id))
            {
                _enemyHScenes.Add(id);
                if (_enemyHScenes.Count == LevelInfo.SpawnableEnemies.Length)
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

        public void ResetSpawnTimer(PlayerData pData)
        {
            pData.SpawnTimer = Random.Range(_info.SpawnIntervalMin, _info.SpawnIntervalMax);
        }

        public void IncreaseSpeed(int id, float value)
        {
            var player = _players[id];
            player.Speed = Mathf.Clamp(player.Speed + (value * _info.SpeedChangeMultiplier), _info.MinSpeed, _info.MaxSpeed);
        }

        public void ResetSpeed(int id)
        {
            var player = _players[id];
            player.Speed = _info.MinSpeed;
        }

        public float GetSpeed(int id) => _players[id].Speed;

        public class PlayerData
        {
            public PlayerController PC;
            public Transform UIProg;
            public Transform Spawner;
            public Transform WallOfTentacles;
            public Camera Cam;

            public float Speed;

            public float Progress;
            public float SpawnTimer;
        }
    }
}
