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

        [Header("Info")]
        [SerializeField]
        private GameInfo _info;

        [Header("UI")]
        [SerializeField]
        private Transform _bossBarGame;
        [SerializeField]
        private Transform _bossBarBoss;

        [SerializeField]
        private RectTransform _progressBossBar, _goalProgressBar, _startProgressBar;

        [SerializeField]
        private GameObject _victoryContainer;

        [SerializeField]
        private GameObject _playerUIProgPrefab, _playerUIDownPrefab;

        [SerializeField]
        private PlayerInputManager _inputManager;

        [SerializeField]
        private GameObject _playerContainerPrefab;

        [SerializeField]
        private GameObject _nextLevel;

        [SerializeField]
        private GameObject _waitingForPlayers;

        [Header("Debug (EDITOR ONLY)")]
        [SerializeField]
        private int _startLevelOverrides;

        private int _levelIndex;

        public LevelInfo LevelInfo => _info.Levels[_levelIndex];

        private const float _gameOverTimerRef = 3f;

        private float _progressBoss;

        private const float _maxSpeedTimerRef = 3f;

        private bool _didWin, _didStart;

        public bool DoesPlayerExists(int playerId)
            => _players.ContainsKey(playerId);

        public bool DidGameEnd(int playerId)
        {
            return _didWin || !_didStart || _players[playerId].DidLost;
        }

        private readonly Dictionary<int, PlayerData> _players = new();

        private ButtplugClient _client;

        private readonly List<int> _enemyHScenes = new();

        public int PlayerCount => _players.Count;

        public bool AreAllNudes => _players.All(x => x.Value.PC.IsTopBodyBroken && x.Value.PC.IsTopBodyBroken);

        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            _levelIndex = _startLevelOverrides;
#endif

            if (LevelInfo.IsBossLevel)
            {
                _bossBarGame.gameObject.SetActive(false);
                _bossBarBoss.gameObject.SetActive(true);
            }
            else
            {
                _bossBarGame.gameObject.SetActive(true);
                _bossBarBoss.gameObject.SetActive(false);
            }

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
            if (!_players.Any() ||!_didStart) return;

            if (!LevelInfo.IsBossLevel)
            {
                _progressBoss += Time.deltaTime * _info.BossSpeed;
                _progressBossBar.transform.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, _progressBoss / _info.DestinationDistance), _progressBossBar.transform.position.y, _progressBossBar.transform.position.z);
            }

            foreach (var keyValue in _players)
            {
                var id = keyValue.Key;
                var player = keyValue.Value;

                player.SpawnTimer -= Time.deltaTime * player.Speed; // Timer depends of which speed we are going to

                if (!DidGameEnd(id))
                {
                    if (!LevelInfo.IsBossLevel)
                    {
                        player.Progress += Time.deltaTime * player.Speed;
                    }

                    if (player.Speed == _info.MaxSpeed)
                    {
                        player.MaxSpeedTimer += Time.deltaTime;
                        if (player.MaxSpeedTimer >= _maxSpeedTimerRef)
                        {
                            player.MaxSpeedTimer = 0f;
                            AchievementManager.Instance.Unlock(AchievementID.MaxSpeed);
                        }
                    }
                    else
                    {
                        player.MaxSpeedTimer = 0f;
                    }

                    if (player.Progress < _progressBoss)
                    {
                        TriggerGameOver(id);
                    }
                    else if (player.Progress >= _info.DestinationDistance)
                    {
                        _didWin = true;

                        var targetPlayer = _players.First().Value;

                        // We base achievements on the first player
                        var hasClothes = targetPlayer.PC.IsFullClothed;
                        var hasPower = targetPlayer.PC.IsFullyPowered;
                        var hasNoHScenes = targetPlayer.PC.GotHScene;

                        if (_players.Count == 4 && _players.All(x => !x.Value.DidLost)) AchievementManager.Instance.Unlock(AchievementID.AllAlive4P);
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
                }

                if (!LevelInfo.IsBossLevel)
                {
                    player.WallOfTentacles.position = new(_progressBoss - player.Progress, player.WallOfTentacles.position.y);
                    player.UIProg.position = new(Mathf.Lerp(_startProgressBar.position.x, _goalProgressBar.position.x, player.Progress / _info.DestinationDistance), player.UIProg.position.y, player.UIProg.position.z);
                }

                if (player.SpawnTimer <= 0)
                {
                    if (LevelInfo.SpawnableEnemies.Any())
                    {
                        var go = Instantiate(LevelInfo.SpawnableEnemies[Random.Range(0, LevelInfo.SpawnableEnemies.Length)], player.Spawner.position, Quaternion.identity);
                        go.GetComponent<EnemyController>().PlayerID = id;
                    }

                    ResetSpawnTimer(player);
                }

                if (player.DidLost && player.GameOverTimer < _gameOverTimerRef)
                {
                    player.GameOverTimer += Time.deltaTime;

                    foreach (var i in player.GameOverImage)
                    {
                        var c = i.color;
                        i.color = new(c.r, c.g, c.b, Mathf.Clamp01(player.GameOverTimer / _gameOverTimerRef));
                    }
                }
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
            _didWin = false;

            foreach (var player in _players.Values)
            {
                player.PC.ResetPlayer();

                var ui = Instantiate(_playerUIProgPrefab, LevelInfo.IsBossLevel ? _bossBarBoss : _bossBarGame);
                if (LevelInfo.IsBossLevel)
                {
                    ui.transform.Translate(Vector2.right * 10f * _players.Count);
                }
                for (int i = 0; i < ui.transform.childCount; i++)
                {
                    ui.transform.GetChild(i).GetComponent<Image>().color = player.PC.Color;
                }
                Destroy(player.UIProg.gameObject);
                player.UIProg = ui.transform;

                player.Speed = _info.MinSpeed;
                player.Progress = 0f;
                player.SpawnTimer = 0f;
                player.DidLost = false;
                player.MaxSpeedTimer = 0f;
            }
        }

        public void RegisterPlayer(Transform enemySpawn, Transform tentacles, PlayerController pc, Camera cam, GameObject gameOverContainer, Image[] gameOverImage)
        {
            var colors = new[]
            {
                new Color(0.4509f, 0.0235f, 0f),
                new Color(0f, 0.4428f, 0.4509f),
                new Color(0.2501f, 0.4509f, 0f),
                new Color(0.2233f, 0f, 0.4509f)
            };
            pc.Color = colors[_players.Count];
            var ui = Instantiate(_playerUIProgPrefab, LevelInfo.IsBossLevel ? _bossBarBoss : _bossBarGame);
            if (LevelInfo.IsBossLevel)
            {
                ui.transform.Translate(Vector2.right * 10f * _players.Count);
            }
            for (int i = 0; i < ui.transform.childCount; i++)
            {
                ui.transform.GetChild(i).GetComponent<Image>().color = pc.Color;
            }
            var data = new PlayerData()
            {
                Cam = cam,
                PC = pc,
                Spawner = enemySpawn,
                WallOfTentacles = tentacles,
                UIProg = ui.transform,
                Speed = _info.MinSpeed,
                Progress = _players.Any() ? _players.Min(x => x.Value.Progress) : 0f,
                SpawnTimer = 0f,
                GameOverContainer = gameOverContainer,
                GameOverImage = gameOverImage,
                GameOverTimer = 0f,
                DidLost = false,
                MaxSpeedTimer = 0f
            };
            _players.Add(pc.gameObject.GetInstanceID(), data);
            ResetSpawnTimer(data);

            UpdateCameras();

            if (_players.Count == GlobalData.PlayerCount)
            {
                _didStart = true;
                _waitingForPlayers.SetActive(false);
            }
        }

        public void UnregisterPlayer(PlayerController pc)
        {
            _players.Remove(pc.gameObject.GetInstanceID());

            UpdateCameras();
        }

        private void UpdateCameras()
        {
            var yMax = _players.Count > 1 ? .5f : 1f;
            var xMax = _players.Count > 2 ? .5f : 1f;
            for (int i = 0; i < _players.Count; i++)
            {
                var y = i % 2;
                var x = i / 2;
                _players.Values.ElementAt(i).Cam.rect = new(xMax * x, yMax * y, xMax, yMax);
            }
        }

        public void HitEnemy(int id)
        {
            _players[id].MaxSpeedTimer = 0f;
        }

        public void PlayHScene(int playerId, int id)
        {
            var player = _players[playerId];

            player.Speed = 0;
            if (!_enemyHScenes.Contains(id))
            {
                _enemyHScenes.Add(id);
                if (_levelIndex == 3 && _enemyHScenes.Count == LevelInfo.SpawnableEnemies.Length)
                {
                    AchievementManager.Instance.Unlock(AchievementID.AllHScenes);
                }
            }
        }

        public void TriggerGameOver(int id)
        {
            var pData = _players[id];
            if (pData.DidLost) return; // Just in case

            pData.DidLost = true;
            pData.GameOverContainer.SetActive(true);

            if (!LevelInfo.IsBossLevel)
            {
                var go = Instantiate(_playerUIDownPrefab, _bossBarGame);
                go.transform.position = pData.UIProg.position;
                go.transform.GetChild(0).GetComponent<Image>().color = pData.PC.Color;
                Destroy(pData.UIProg.gameObject);
                pData.UIProg = go.transform;
            }
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

            public GameObject GameOverContainer;
            public Image[] GameOverImage;
            public float GameOverTimer;

            public bool DidLost;

            public float Speed;

            public float Progress;
            public float SpawnTimer;

            public float MaxSpeedTimer;
        }
    }

    public enum GameOverState
    {
        None,
        Victory,
        Lost
    }
}
