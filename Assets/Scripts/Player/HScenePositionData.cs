using UnityEngine;

namespace FlashSexJam.Player
{
    public class HScenePositionData : MonoBehaviour
    {
        [SerializeField]
        private GameObject _upperBodyCloth, _lowerBodyCloth;

        public void InitClothes(PlayerController pc)
        {
            if (pc.IsTopBodyBroken) Destroy(_upperBodyCloth);
            else _upperBodyCloth.GetComponent<SpriteRenderer>().color = pc.Color;
            if (pc.IsLowerBodyBroken) Destroy(_lowerBodyCloth);
            else _lowerBodyCloth.GetComponent<SpriteRenderer>().color = pc.Color;
        }
    }
}
