using FlashSexJam.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam
{
    public class PlayerController : MonoBehaviour
    {
        private float _xMov;

        private void Update()
        {
            GameManager.Instance.IncreaseSpeed(_xMov * Time.deltaTime);
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            var y = 0f;
            if (mov.y > 0f) y = 3f;
            else if (mov.y < 0f) y = -3f;

            transform.position = new(transform.position.x, y);

            _xMov = mov.x;
        }
    }

}