﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicController : MonoBehaviour
{
    public GuaranteeSingleSpawn SingleSpawnCheck;
    public AudioSource AudioSource;
    public VolumeFader VolumeFader;
    public int FadeOutDuration = 48;
    public bool NoMusicInEditor = false;
    public MusicEntry[] MusicEntries;

    [System.Serializable]
    public struct MusicEntry
    {
        public string SceneName;
        public AudioClip Clip;
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (this.NoMusicInEditor)
        {
            this.AudioSource.Stop();
            this.AudioSource.enabled = false;
            this.enabled = false;
            return;
        }
#endif
        if (!this.SingleSpawnCheck.MarkedForDestruction)
        {
            SceneManager.sceneLoaded += onSceneChanged;
        }
    }

    /**
     * Private
     */
    private Dictionary<string, AudioClip> _musicDict;

    private void beginSceneTransition(LocalEventNotifier.Event e)
    {
        string nextSceneName = (e as BeginSceneTransitionEvent).NextSceneName;
        if (_musicDict.ContainsKey(nextSceneName) && _musicDict[nextSceneName] != this.AudioSource.clip)
            this.VolumeFader.BeginFade(this.FadeOutDuration, 0.0f);
    }

    private void compileMusicDict()
    {
        _musicDict = new Dictionary<string, AudioClip>(this.MusicEntries.Length);

        for (int i = 0; i < this.MusicEntries.Length; ++i)
        {
            _musicDict.Add(this.MusicEntries[i].SceneName, this.MusicEntries[i].Clip);
        }
    }

    private void onSceneChanged(Scene scene, LoadSceneMode lodeMode)
    {
        if (!this.SingleSpawnCheck.MarkedForDestruction)
        {
            if (_musicDict == null)
            {
                compileMusicDict();
            }
            GlobalEvents.Notifier.Listen(BeginSceneTransitionEvent.NAME, this, beginSceneTransition);

            this.AudioSource.volume = 1.0f;
            string sceneName = SceneManager.GetActiveScene().name;
            if (_musicDict.ContainsKey(sceneName) && this.AudioSource.clip != _musicDict[sceneName])
            {
                this.AudioSource.clip = _musicDict[sceneName];
                this.AudioSource.Play();
            }
        }
    }
}

public class BeginSceneTransitionEvent : LocalEventNotifier.Event
{
    public const string NAME = "SCENE_TRANSITION";
    public string NextSceneName;

    public BeginSceneTransitionEvent(string nextSceneName)
    {
        this.Name = NAME;
        this.NextSceneName = nextSceneName;
    }
}
