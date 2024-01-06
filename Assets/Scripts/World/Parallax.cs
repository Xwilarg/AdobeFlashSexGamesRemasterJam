using UnityEngine;

namespace FlashSexJam.World
{
    public class Parallax : MonoBehaviour
    {
        private void Update()
        {
            transform.Translate(Vector3.left * Time.deltaTime);
            if (transform.position.x < -20f)
            {
                transform.Translate(Vector3.right * 40f);
            }
        }
    }
}
