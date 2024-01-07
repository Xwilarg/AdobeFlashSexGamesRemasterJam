using UnityEngine;

namespace FlashSexJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        public float MinSpeed, MaxSpeed;

        public float SpeedChangeMultiplier;

        public float SpawnIntervalMin, SpawnIntervalMax;

        public GameObject[] SpawnableEnemies;
    }
}