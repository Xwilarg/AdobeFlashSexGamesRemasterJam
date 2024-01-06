using FlashSexJam.SO;
using UnityEngine;

namespace FlashSexJam.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField]
        private GameInfo _info;

        public float Speed { private set; get; }

        private void Awake()
        {
            Instance = this;
            Speed = _info.MinSpeed;
        }

        public void IncreaseSpeed(float value)
        {
            Speed = Mathf.Clamp(Speed + (value * _info.SpeedChangeMultiplier), _info.MinSpeed, _info.MaxSpeed);
        }
    }
}
