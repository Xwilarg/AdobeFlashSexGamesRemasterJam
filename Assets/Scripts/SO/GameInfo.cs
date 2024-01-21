using UnityEngine;

namespace FlashSexJam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        public float MinSpeed, MaxSpeed;

        public float SpeedChangeMultiplier;

        public float SpawnIntervalMin, SpawnIntervalMax;

        public float DestinationDistance;

        public float BossSpeed;
        public float BossNegativeOffset;

        public LevelInfo[] Levels;
    }

    [System.Serializable]
    public class LevelInfo
    {
        public GameObject[] SpawnableEnemies;
    }
}