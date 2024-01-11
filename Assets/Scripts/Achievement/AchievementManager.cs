using FlashSexJam.Persistency;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlashSexJam.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private GameObject _prefab;

        public static AchievementManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Unlock(AchievementID achievement)
        {
            if (PersistencyManager.Instance.SaveData.IsUnlocked(achievement))
            {
                return;
            }
            var instance = Instantiate(_prefab, _container);

            var data = Achievements[achievement];
            instance.GetComponentInChildren<TMP_Text>().text = data.Name;

            PersistencyManager.Instance.SaveData.Unlock(achievement);
            PersistencyManager.Instance.Save();

            Destroy(instance, 5f);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.Victory, new() { Name = "One step toward purity", Description = "Complete a level" } },
            { AchievementID.VictoryNoHScene, new() { Name = "Stainless", Description = "Complete a level without triggering any lewd scene" } },
            { AchievementID.VictoryNoClothDamage, new() { Name = "Top fashion", Description = "Complete a level without loosing any cloth" } },
            { AchievementID.VictoryFullPower, new() { Name = "3% of my real power", Description = "Complete a level without using your power" } },
            { AchievementID.VictoryPerfect, new() { Name = "Katabasis", Description = "Complete a level without triggering any lewd scene nor loosing cloth, and without using your power" } }
        };
    }

    public enum AchievementID
    {
        Victory,
        VictoryNoHScene,
        VictoryNoClothDamage,
        VictoryFullPower,
        VictoryPerfect
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}