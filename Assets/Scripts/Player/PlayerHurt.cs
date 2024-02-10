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

        public void DeleteDamageAnim(BodyPartType type)
        {
            if (type == BodyPartType.UpperBody) _upperAnim.gameObject.SetActive(false);
            else if (type == BodyPartType.LowerBody) _lowerAnim.gameObject.SetActive(false);
        }

        public void ResetClothes()
        {
            _upperAnim.gameObject.SetActive(false);
            _upperAnim.enabled = false;
            _lowerAnim.gameObject.SetActive(false);
            _lowerAnim.enabled = false;
        }
    }
}
