using FlashSexJam.Achievement;
using FlashSexJam.Manager;
using FlashSexJam.SO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FlashSexJam.Player
{
    public class HSceneController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        [SerializeField]
        private RectTransform _orgasmBar, _energyBar;
        private Image _orgasmBarImage;

        [SerializeField]
        private Image _orgasmHeart;

        private PlayerController _pc;
        private float _strokeCount;

        private GameObject _hSceneObj;

        private float _orgasm, _energy;

        private bool _isOrgasming;

        private void Awake()
        {
            _pc = GetComponentInChildren<PlayerController>();
            _pc.HScene = this;

            _orgasmBarImage = _orgasmBar.GetComponent<Image>();

            _orgasm = _info.BaseOrgasm;
            _energy = _info.BaseEnergy;

            UpdateUI();
        }

        private void Update()
        {
            if (_strokeCount > 0f && !GameManager.Instance.DidGameEnd(_pc.PlayerID))
            {
                _strokeCount -= Time.deltaTime;
                _orgasm -= Time.deltaTime;

                if (_orgasm <= 0f && !_isOrgasming)
                {
                    _isOrgasming = true;
                    StartCoroutine(PlayOrgasmVfx());
                }

                UpdateUI();

                if (_strokeCount <= 0f)
                {
                    StartCoroutine(StopHScene());
                }
            }
        }

        private void UpdateUI()
        {
            _orgasmBar.localScale = new(Mathf.Clamp01(1f - (_orgasm / _info.BaseOrgasm)), 1f, 1f);
            _energyBar.localScale = new(Mathf.Clamp01(_energy / _info.BaseEnergy), 1f, 1f);
        }

        private IEnumerator PlayOrgasmVfx()
        {
            var pink = new Color(0.9811321f, 0.1990032f, 0.8889886f);
            for (int i = 0; i < 6; i++)
            {
                _orgasmHeart.color = i % 2 == 0 ? pink : Color.red;
                _orgasmBarImage.color = _orgasmHeart.color;
                yield return new WaitForSeconds(.2f);
            }

            _orgasm = _info.BaseOrgasm;
            _energy -= _info.BaseEnergy / 2f;
            _isOrgasming = false;
            _orgasmHeart.color = Color.white;
            _orgasmBarImage.color = pink;
            if (_energy <= 0f)
            {
                GameManager.Instance.TriggerGameOver(_pc.PlayerID);
            }
        }

        public void PlayHScene(GameObject hSceneObj, int id)
        {
            _hSceneObj = hSceneObj;

            GameManager.Instance.PlayHScene(_pc.PlayerID, id);
            _pc.gameObject.SetActive(false);
            _strokeCount = Random.Range(20, 30);

            _pc.GotHScene = true;
        }

        public IEnumerator StopHScene()
        {
            while (_isOrgasming)
            {
                yield return new WaitForEndOfFrame();
            }

            if (_orgasm / _info.BaseOrgasm <= .05f)
            {
                AchievementManager.Instance.Unlock(AchievementID.Edging);
            }

            GameManager.Instance.ResetSpeed(_pc.PlayerID);

            _pc.gameObject.SetActive(true);
            _pc.ToggleInvulnerabilityFrames();

            StopHSceneNow();

        }

        public void StopHSceneNow()
        {
            Destroy(_hSceneObj);
        }

        public void OnStoke(InputAction.CallbackContext value)
        {
            if (value.performed && _strokeCount > 0f && !GameManager.Instance.DidGameEnd(_pc.PlayerID))
            {
                _strokeCount--;

                if (_strokeCount <= 0f)
                {
                    StartCoroutine(StopHScene());
                }
            }
        }
    }
}
