using UnityEngine;

namespace FlashSexJam.Player
{
    public class HScenePositionData : MonoBehaviour
    {
        [SerializeField]
        private GameObject _upperBodyCloth, _lowerBodyCloth;

        public void BreakClothes(PlayerController pc)
        {
            if (pc.IsTopBodyBroken) Destroy(_upperBodyCloth);
            if (pc.IsLowerBodyBroken) Destroy(_lowerBodyCloth);
        }
    }
}
