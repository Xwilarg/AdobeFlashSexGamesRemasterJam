using FlashSexJam.SO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FlashSexJam
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        public void OnMove(InputAction.CallbackContext value)
        {
            var mov = value.ReadValue<Vector2>();

            var y = 0f;
            if (mov.y > 0f) y = 2f;
            else if (mov.y < 0f) y = -2f;

            transform.position = new(transform.position.x, y);
        }
    }

}