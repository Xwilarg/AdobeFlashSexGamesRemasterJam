using UnityEngine;

namespace FlashSexJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class PlayerInfo : ScriptableObject
    {
        public float BaseEnergy, BaseOrgasm;
    }
}