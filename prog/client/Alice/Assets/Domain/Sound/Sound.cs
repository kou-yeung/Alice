using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Sound
{
    public interface ISound
    {
        void PlayBGM(string path);
        void PlaySE(string path);
    }

    /// <summary>
    /// Soundサービスを提供する
    /// </summary>
    public class SoundService : ServiceLocator<ISound> { }
}

