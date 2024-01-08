using UnityEngine;

namespace FlashSexJam.Player
{
    public class PositionData : MonoBehaviour
    {
        [SerializeField]
        private GameObject _upperBodyCloth, _lowerBodyCloth;

        [SerializeField]
        private BodyPart _upperBody, _lowerBody;

        public (GameObject UpperCloth, GameObject LowerCloth) Init(PlayerController pc)
        {
            _upperBody.Owner = pc;
            _upperBody.Type = BodyPartType.UpperBody;
            _lowerBody.Owner = pc;
            _lowerBody.Type = BodyPartType.LowerBody;

            return (_upperBodyCloth, _lowerBodyCloth);
        }
    }
}
