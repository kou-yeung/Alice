
using UnityEngine;

namespace Zoo.Sound
{
    public class SoundMute : ISound
    {
        public void PlayBGM(string path)
        {
            Debug.Log($"PlayBGM:{path}");
        }
        public void PlaySE(string path)
        {
            Debug.Log($"PlaySE:{path}");
        }
    }
}

