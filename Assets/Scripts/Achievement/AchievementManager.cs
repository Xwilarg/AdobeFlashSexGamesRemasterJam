using FlashSexJam.Persistency;
using FlashSexJam.Translation;
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
            instance.GetComponentInChildren<TMP_Text>().text = Translate.Instance.Tr(data.Name);

            PersistencyManager.Instance.SaveData.Unlock(achievement);
            PersistencyManager.Instance.Save();

            Destroy(instance, 5f);
        }

        public Dictionary<AchievementID, Achievement> Achievements { get; } = new()
        {
            { AchievementID.VictoryNoHScene, new() { Name = "achVictoryNoHSceneName", Description = "achVictoryNoHSceneDesc" } },
            { AchievementID.VictoryNoClothDamage, new() { Name = "achVictoryNoClothDamageName", Description = "achVictoryNoClothDamageDesc" } },
            { AchievementID.VictoryFullPower, new() { Name = "achVictoryFullPowerName", Description = "achVictoryFullPowerDesc" } },
            { AchievementID.VictoryPerfect, new() { Name = "achVictoryPerfectName", Description = "achVictoryPerfectDesc" } },

            { AchievementID.AllAlive4P, new() { Name = "achAllAlive4PName", Description = "achAllAlive4PDesc" } },

            { AchievementID.MaxSpeed, new() { Name = "achMaxSpeedName", Description = "achMaxSpeedDesc" } },
            { AchievementID.AllHScenes, new() { Name = "achAllHScenesName", Description = "achAllHScenesDesc" } },
            { AchievementID.Edging, new() { Name = "achEdgingName", Description = "achEdgingDesc" } },
            { AchievementID.Credits, new() { Name = "achCreditsName", Description = "achCreditsDesc" } }
        };
    }

    public enum AchievementID
    {
        VictoryNoHScene,
        VictoryNoClothDamage,
        VictoryFullPower,
        VictoryPerfect,

        MaxSpeed,

        AllHScenes,
        Edging,

        Credits,

        AllAlive4P
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}