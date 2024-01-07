using FlashSexJam.Manager;
using UnityEngine;

namespace FlashSexJam.World
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField]
        private float _speedMultiplier = 1f;

        private void Update()
        {
            transform.Translate(Vector3.left * Time.deltaTime * GameManager.Instance.Speed * _speedMultiplier);
            if (transform.position.x < -20f)
            {
                transform.Translate(Vector3.right * 40f);
            }
        }
    }
}
