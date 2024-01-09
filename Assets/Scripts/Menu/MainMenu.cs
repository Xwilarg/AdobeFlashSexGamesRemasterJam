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
    }
}
