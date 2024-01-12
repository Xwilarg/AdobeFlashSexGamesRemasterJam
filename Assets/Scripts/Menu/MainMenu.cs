using FlashSexJam.Achievement;
using FlashSexJam.Translation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlashSexJam.Menu
{
    public class MainMenu : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);
        }

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        public void SetLanguage()
        {
            if (Translate.Instance.CurrentLanguage == "english")
                Translate.Instance.CurrentLanguage = "french";
            else
                Translate.Instance.CurrentLanguage = "english";
        }

        public void GetCreditsAchievement()
        {
            AchievementManager.Instance.Unlock(AchievementID.Credits);
        }
    }
}
