using FlashSexJam.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class HSceneController : MonoBehaviour
    {
        private PlayerController _pc;
        private int _strokeCount;

        private GameObject _hSceneObj;

        private void Awake()
        {
            _pc = GetComponentInChildren<PlayerController>();
            _pc.HScene = this;
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
            if (value.performed && _strokeCount > 0)
            {
                _strokeCount--;

                if (_strokeCount == 0)
                {
                    StopHScene();
                }
            }
        }
    }
}
