using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using Zoo.IO;
using System.Linq;

namespace Zoo.Sound
{
    public class SoundClip : ISound
    {
        GameObject audioSources;

        AudioSource bgmSource;
        List<AudioSource> seSources = new List<AudioSource>();

        public SoundClip(int maxSe = 64)
        {
            audioSources = new GameObject("AudioSources");
            GameObject.DontDestroyOnLoad(audioSources);

            // AudioSourceを作成しておく : BGM
            bgmSource = audioSources.AddComponent<AudioSource>();
            bgmSource.loop = true;

            // AudioSourceを作成しておく : SE
            for (int i = 0; i < maxSe; i++)
            {
                var source = audioSources.AddComponent<AudioSource>();
                seSources.Add(source);
            }
        }

        public void PlayBGM(string path)
        {
            LoaderService.Instance.LoadAsync<AudioClip>(path, (clip) =>
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            });
        }

        public void PlaySE(string path)
        {
            LoaderService.Instance.LoadAsync<AudioClip>(path, (clip) =>
            {
                var source = seSources.FirstOrDefault(v => !v.isPlaying);
                if(source != null)
                {
                    source.clip = clip;
                    source.Play();
                }
            });
        }
    }
}
