using UnityEngine;

namespace FlashSexJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/LevelInfo", fileName = "LevelInfo")]
    public class LevelInfo : ScriptableObject
    {
        public GameObject[] SpawnableEnemies;
        public bool IsBossLevel;
    }
}
