using UnityEngine;

namespace FlashSexJam.Player
{
    public class RotateFade : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(0, 0, 0.15f * Time.deltaTime, Space.Self);
        }
    }
}
