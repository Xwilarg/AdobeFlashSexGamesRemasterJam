using UnityEngine;

namespace FlashSexJam.Player
{
    public class Cloth : MonoBehaviour
    {
        [SerializeField]
        private BodyPartType _owner;
        public BodyPartType Owner => _owner;
    }
}
