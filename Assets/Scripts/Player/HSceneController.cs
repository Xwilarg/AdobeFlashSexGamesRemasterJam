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

            _orgasm = _info.BaseOrgasm;
            _energy = _info.BaseEnergy;

            UpdateUI();
        }

        private void Update()
        {
            if (_strokeCount > 0f && !GameManager.Instance.DidGameEnd)
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
                    StopHScene();
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
            for (int i = 0; i < 6; i++)
            {
                _orgasmHeart.color = i % 2 == 0 ? new Color(0.9811321f, 0.1990032f, 0.8889886f) : Color.red;
                yield return new WaitForSeconds(.1f);
            }

            _orgasm = _info.BaseOrgasm;
            _energy -= _info.BaseEnergy / 2f;
            _isOrgasming = false;
            _orgasmHeart.color = Color.white;
            if (_energy <= 0f)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }

        public void PlayHScene(GameObject hSceneObj, int id)
        {
            _hSceneObj = hSceneObj;

            GameManager.Instance.PlayHScene(id);
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
            GameManager.Instance.ResetSpeed();
            _pc.gameObject.SetActive(true);

            _pc.ToggleInvulnerabilityFrames();

            Destroy(_hSceneObj);
        }

        public void OnStoke(InputAction.CallbackContext value)
        {
            if (value.performed && _strokeCount > 0f && !GameManager.Instance.DidGameEnd)
            {
                _strokeCount--;

                if (_strokeCount <= 0f)
                {
                    StopHScene();
                }
            }
        }
    }
}
