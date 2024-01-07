using FlashSexJam.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        private float _movYOffset = 2f;

        [SerializeField]
        private GameObject _modelUp, _modelMid, _modelDown;

        private void Awake()
        {
            foreach (var bp in GetComponentsInChildren<BodyPart>())
            {
                bp.Owner = this;
            }
        }

        private void Update()
        {
            GameManager.Instance.IncreaseSpeed(_xMov * Time.deltaTime);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            _modelUp.SetActive(false);
            _modelMid.SetActive(false);
            _modelDown.SetActive(false);

            if (mov.y > 0) _modelUp.SetActive(true);
            else if (mov.y < 0) _modelDown.SetActive(true);
            else _modelMid.SetActive(true);

            _xMov = mov.x;
        }
    }

}