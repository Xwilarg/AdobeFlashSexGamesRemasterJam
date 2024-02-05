using UnityEngine;

namespace FlashSexJam.Player
{
    public class PlayerHurt : MonoBehaviour
    {
        [SerializeField]
        private Animator _upperAnim, _lowerAnim;

        public void PlayDamageAnim(BodyPartType type)
        {
            if (type == BodyPartType.UpperBody) _upperAnim.enabled = true;
            else if (type == BodyPartType.LowerBody) _lowerAnim.enabled = true;
        }
    }
}
