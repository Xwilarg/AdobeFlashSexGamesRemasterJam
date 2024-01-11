using FlashSexJam.Persistency;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FlashSexJam.Achievement
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _achievementPanel;

        [SerializeField]
        private TMP_Text _title;

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
            var data = Achievements[achievement];
            _title.text = data.Name;
            _achievementPanel.SetActive(true);

            PersistencyManager.Instance.SaveData.Unlock(achievement);
            PersistencyManager.Instance.Save();

            StartCoroutine(WaitAndClosePopup());
        }

        private IEnumerator WaitAndClosePopup()
        {
            yield return new WaitForSeconds(5f);
            _achievementPanel.SetActive(false);
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