using FlashSexJam.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        private float _movYOffset = 2f;

        private void Update()
        {
            GameManager.Instance.IncreaseSpeed(_xMov * Time.deltaTime);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            var y = 0f;
            if (mov.y > 0f) y = _movYOffset;
            else if (mov.y < 0f) y = -_movYOffset;

            transform.position = new(transform.position.x, y);

            _xMov = mov.x;
        }
    }

}