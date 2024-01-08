using FlashSexJam.Manager;
using FlashSexJam.SO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class HSceneController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        [SerializeField]
        private RectTransform _orgasmBar, _energyBar;

        private PlayerController _pc;
        private float _strokeCount;

        private GameObject _hSceneObj;

        private float _orgasm, _energy;

        private void Awake()
        {
            _pc = GetComponentInChildren<PlayerController>();
            _pc.HScene = this;

            _orgasm = _info.BaseOrgasm;
            _energy = _info.BaseEnergy;
        }

        private void Update()
        {
            if (_strokeCount > 0f)
            {
                _strokeCount -= Time.deltaTime;
                _orgasm -= Time.deltaTime;

                if (_orgasm <= 0f)
                {
                    _orgasm = _info.BaseOrgasm;
                    _energy -= _info.BaseEnergy / 2f;

                    if (_energy <= 0f)
                    {
                        // TODO: Game Over
                    }
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
            _orgasmBar.localScale = new(_orgasm / _info.BaseOrgasm, 1f, 1f);
            _energyBar.localScale = new(_energy / _info.BaseEnergy, 1f, 1f);
        }

        public void PlayHScene(GameObject hSceneObj)
        {
            _hSceneObj = hSceneObj;

            GameManager.Instance.StopSpeed();
            _pc.gameObject.SetActive(false);
            _strokeCount = Random.Range(20, 30);
        }

        public void StopHScene()
        {
            GameManager.Instance.ResetSpeed();
            _pc.gameObject.SetActive(true);

            Destroy(_hSceneObj);
        }

        public void OnStoke(InputAction.CallbackContext value)
        {
            if (value.performed && _strokeCount > 0f)
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
