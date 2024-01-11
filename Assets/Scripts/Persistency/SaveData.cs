using FlashSexJam.Achievement;
using System.Collections.Generic;

namespace FlashSexJam.Persistency
{
    public class SaveData
    {
        public List<AchievementID> UnlockedAchievements { set; get; } = new();

        public bool IsUnlocked(AchievementID id)
            => UnlockedAchievements.Contains(id);

        public void Unlock(AchievementID id)
        {
            UnlockedAchievements.Add(id);
            PersistencyManager.Instance.Save();
        }
    }
}