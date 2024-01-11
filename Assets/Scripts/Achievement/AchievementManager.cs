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
        };
    }

    public enum AchievementID
    {
    }

    public record Achievement
    {
        public string Name;
        public string Description;
    }
}