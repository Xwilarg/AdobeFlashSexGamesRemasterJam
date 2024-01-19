using FlashSexJam.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        public HSceneController HScene { set; get; }

        [SerializeField]
        private PositionData _modelUp, _modelMid, _modelDown;

        [SerializeField]
        private GameObject _positionContainers;

        [SerializeField]
        private GameObject _attackPrefab;

        [SerializeField]
        private TMP_Text _attackCountText;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private Transform _wallOfTentacles;

        [SerializeField]
        private Camera _cam;

        private int _attackCount = 3;

        public bool IsInvulnerable { private set; get; }

        public bool IsFullClothed => _clothes[BodyPartType.LowerBody].Any() && _clothes[BodyPartType.UpperBody].Any();
        public bool IsFullyPowered => _attackCount == 3;
        public bool GotHScene { set; get; }

        private readonly Dictionary<BodyPartType, List<GameObject>> _clothes = new()
        {
            { BodyPartType.Head, new() },
            { BodyPartType.UpperBody, new() },
            { BodyPartType.LowerBody, new() }
        };

        private void Awake()
        {
            var models = new[] { _modelUp, _modelMid, _modelDown };
            foreach (var m in models)
            {
                var (UpperCloth, LowerCloth) = m.Init(this);
                _clothes[BodyPartType.UpperBody].Add(UpperCloth);
                _clothes[BodyPartType.LowerBody].Add(LowerCloth);
            }

            _attackCountText.text = _attackCount.ToString();
        }

        private void Start()
        {
            GameManager.Instance.RegisterPlayer(_spawnPoint, _wallOfTentacles, this, _cam);
        }

        private void OnDestroy()
        {
            GameManager.Instance.UnregisterPlayer(this);
        }

        private void Update()
        {
            if (!GameManager.Instance.DidGameEnd)
            {
                GameManager.Instance.IncreaseSpeed(_xMov * Time.deltaTime);
            }
        }

        /// <returns>false if already naked</returns>
        public bool TryBreakCloth(BodyPartType bodyPart)
        {
            if (!_clothes[bodyPart].Any())
            {
                return false;
            }
            foreach (var c in _clothes[bodyPart])
            {
                Destroy(c);
            }
            _clothes[bodyPart].Clear();
            return true;
        }

        public bool IsTopBodyBroken => !_clothes[BodyPartType.UpperBody].Any();
        public bool IsLowerBodyBroken => !_clothes[BodyPartType.LowerBody].Any();

        public void ToggleInvulnerabilityFrames()
        {
            IsInvulnerable = true;
            StartCoroutine(PlayInvulnerabilityFrames());
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
            if (GameManager.Instance.DidGameEnd) return;

            var mov = value.ReadValue<Vector2>();

            _modelUp.gameObject.SetActive(false);
            _modelMid.gameObject.SetActive(false);
            _modelDown.gameObject.SetActive(false);

            if (mov.y > 0) _modelUp.gameObject.SetActive(true);
            else if (mov.y < 0) _modelDown.gameObject.SetActive(true);
            else _modelMid.gameObject.SetActive(true);

            _xMov = mov.x;
        }

        public void OnAttack(InputAction.CallbackContext value)
        {
            if (GameManager.Instance.DidGameEnd) return;

            if (value.performed && !IsInvulnerable && gameObject.activeInHierarchy && _attackCount > 0)
            {
                _attackCount--;
                _attackCountText.text = _attackCount.ToString();

                var bounds = CalculateBounds();
                var atk = Instantiate(_attackPrefab, new Vector2(bounds.min.x, 0f), Quaternion.identity);
                atk.GetComponent<PlayerAttack>().MaxX = bounds.max.x;
            }
        }
    }
}