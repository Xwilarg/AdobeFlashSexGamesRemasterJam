using FlashSexJam.Translation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlashSexJam.Menu
{
    public class MainMenu : MonoBehaviour
    {
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
    }
}
