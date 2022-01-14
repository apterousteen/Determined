using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    MenuController menuController;

    [Header("Audio Sprites")]
    [SerializeField] public Sprite mutedSprite = null;
    [SerializeField] public Sprite unmutedSprite = null;
    [SerializeField] public Sprite mutedSpriteMini = null;
    [SerializeField] public Sprite unmutedSpriteMini = null;
    [SerializeField] public Toggle toggle = null;

    public bool muted;

    public Sound[] sounds;

    private void Awake()
    {
        menuController = FindObjectOfType<MenuController>();

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        foreach(var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    private void Update()
    {
        if (muted)
        {
            AudioListener.volume = 0;
        }
        else
        {
            menuController.Unmute();
        }
    }

    private void Start()
    {
        Play("Theme");
    }

    public void Play(string name)
    {
        var sound = Array.Find(sounds, sound => sound.name == name);
        sound.source.Play();
    }
}
