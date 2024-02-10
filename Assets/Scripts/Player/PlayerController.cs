using FlashSexJam.Achievement;
using FlashSexJam.Enemy;
using FlashSexJam.Enemy.Boss;
using FlashSexJam.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FlashSexJam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        public HSceneController HScene { set; get; }

        [SerializeField]
        private PositionData _modelUp, _modelMid, _modelDown;

        [SerializeField]
        private PlayerHurt _hurt;

        [SerializeField]
        private GameObject _positionContainers;

        [SerializeField]
        private GameObject _attackPrefab, _grenadePrefab;

        [SerializeField]
        private TMP_Text _attackCountText;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private Transform _wallOfTentacles;

        [SerializeField]
        private Camera _cam;

        [SerializeField]
        private Transform _parentContainer;

        [SerializeField]
        private GameObject _gameOverContainer;

        [SerializeField]
        private Image[] _gameOverImage;

        [SerializeField]
        private NunBoss _boss;

        public Color Color { set; get; }

        private int _attackCount;
        public void GetAttackPowerup()
        {
            _attackCount++;
            _attackCountText.text = _attackCount.ToString();
        }

        public bool IsInvulnerable { private set; get; }

        public bool IsFullClothed => !IsTopBodyBroken && !IsLowerBodyBroken;
        public bool IsFullyPowered => _attackCount == 3;
        public bool GotHScene { set; get; }

        public int PlayerID => gameObject.GetInstanceID();

        private readonly Dictionary<BodyPartType, List<GameObject>> _clothes = new()
        {
            { BodyPartType.Head, new() },
            { BodyPartType.UpperBody, new() },
            { BodyPartType.LowerBody, new() }
        };

        private void Start()
        {
            if (!GameManager.Instance.LevelInfo.IsBossLevel)
            {
                _attackCount = 3;
            }
            _attackCountText.text = _attackCount.ToString();

            _parentContainer.Translate(Vector2.up * 100f * GameManager.Instance.PlayerCount);
            GameManager.Instance.RegisterPlayer(_spawnPoint, _wallOfTentacles, this, _cam, _gameOverContainer, _gameOverImage, _boss);

            var models = new[] { _modelUp, _modelMid, _modelDown };
            foreach (var m in models)
            {
                var (UpperCloth, LowerCloth) = m.Init(this);
                _clothes[BodyPartType.UpperBody].Add(UpperCloth);
                _clothes[BodyPartType.LowerBody].Add(LowerCloth);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnregisterPlayer(this);
        }

        private void Update()
        {
            if (!GameManager.Instance.DidGameEnd(PlayerID))
            {
                GameManager.Instance.IncreaseSpeed(PlayerID, _xMov * Time.deltaTime);
            }
        }

        public void ResetPlayer()
        {
            gameObject.SetActive(true);
            HScene.StopHSceneNow();

            _modelUp.gameObject.SetActive(false);
            _modelMid.gameObject.SetActive(true);
            _modelDown.gameObject.SetActive(false);

            _hurt.ResetClothes();

            foreach (var c in _clothes)
            {
                foreach (var v in c.Value)
                {
                    v.SetActive(true);
                }
            }
            _attackCount = 3;
        }

        /// <returns>false if already naked</returns>
        public bool TryBreakCloth(BodyPartType bodyPart)
        {
            if (!_clothes[bodyPart].Any(x => x.activeInHierarchy))
            {
                return false;
            }
            foreach (var c in _clothes[bodyPart])
            {
                c.SetActive(false);
            }
            if (GameManager.Instance.PlayerCount == 4 && GameManager.Instance.AreAllNudes)
            {
                AchievementManager.Instance.Unlock(AchievementID.AllNude4P);
            }
            return true;
        }

        public bool IsTopBodyBroken => !_clothes[BodyPartType.UpperBody].Any(x => x.activeInHierarchy);
        public bool IsLowerBodyBroken => !_clothes[BodyPartType.LowerBody].Any(x => x.activeInHierarchy);

        public void ToggleClothDamage(BodyPartType type)
        {
            IsInvulnerable = true;
            StartCoroutine(PlayHurtAnim(type));
        }

        public void ToggleInvulnerabilityFrames()
        {
            IsInvulnerable = true;
            StartCoroutine(PlayInvulnerabilityFrames());
        }

        private IEnumerator PlayHurtAnim(BodyPartType type)
        {
            _hurt.gameObject.SetActive(true);
            _positionContainers.SetActive(false);
            _hurt.PlayDamageAnim(type);

            yield return new WaitForSeconds(.8f);

            _hurt.DeleteDamageAnim(type);
            _hurt.gameObject.SetActive(false);
            _positionContainers.SetActive(true);

            yield return PlayInvulnerabilityFrames();
        }

        private IEnumerator PlayInvulnerabilityFrames()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(.3f);
                _positionContainers.SetActive(!_positionContainers.activeInHierarchy);
            }
            _positionContainers.SetActive(true);
            IsInvulnerable = false;
        }

        private Bounds CalculateBounds()
        {
            float screenAspect = Screen.width / (float)Screen.height;
            float cameraHeight = _cam.orthographicSize * 2;
            Bounds bounds = new(
                _cam.transform.position,
                new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
            return bounds;
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            if (!GameManager.Instance.DoesPlayerExists(PlayerID) || GameManager.Instance.DidGameEnd(PlayerID)) return;

            var mov = value.ReadValue<Vector2>();

            _modelUp.gameObject.SetActive(false);
            _modelMid.gameObject.SetActive(false);
            _modelDown.gameObject.SetActive(false);

            if (mov.y > .5f) _modelUp.gameObject.SetActive(true);
            else if (mov.y < -.5f) _modelDown.gameObject.SetActive(true);
            else _modelMid.gameObject.SetActive(true);

            _xMov = Mathf.Abs(mov.y) <= .5f ? mov.x : 0f; // We can only change speed on base position
        }

        public void OnAttack(InputAction.CallbackContext value)
        {
            if (!GameManager.Instance.DoesPlayerExists(PlayerID) || GameManager.Instance.DidGameEnd(PlayerID)) return;

            if (value.performed && !IsInvulnerable && gameObject.activeInHierarchy && _attackCount > 0)
            {
                if (GameManager.Instance.LevelInfo.IsBossLevel)
                {
                    float y = 0;
                    if (_modelUp.gameObject.activeInHierarchy) y = _modelUp.transform.position.y;
                    else if (_modelDown.gameObject.activeInHierarchy) y = _modelDown.transform.position.y;
                    else y = _modelMid.transform.position.y;

                    Instantiate(_grenadePrefab, new Vector2(transform.position.x, y), Quaternion.identity);
                }
                else
                {
                    var bounds = CalculateBounds();
                    var atk = Instantiate(_attackPrefab, new Vector2(bounds.min.x, 0f), Quaternion.identity);
                    atk.GetComponent<PlayerAttack>().MaxX = bounds.max.x;
                }

                _attackCount--;
                _attackCountText.text = _attackCount.ToString();
            }
        }
    }
}