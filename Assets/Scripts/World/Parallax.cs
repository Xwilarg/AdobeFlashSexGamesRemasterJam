using FlashSexJam.Manager;
using FlashSexJam.Player;
using UnityEngine;

namespace FlashSexJam.World
{
    public class Parallax : MonoBehaviour
    {
        [SerializeField]
        private float _speedMultiplier = 1f;

        [SerializeField]
        private PlayerController _attachedPlayer;

        private void Update()
        {
            transform.Translate(Vector3.left * Time.deltaTime * GameManager.Instance.GetSpeed(_attachedPlayer.PlayerID) * _speedMultiplier);
            if (transform.position.x < -30f)
            {
                transform.Translate(Vector3.right * 60f);
            }
        }
    }
}
