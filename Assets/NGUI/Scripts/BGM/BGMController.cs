﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class BGMController : MonoBehaviour
{
    public string soundFilePath;
    public AudioSource audioSource;
    AudioClip audioClip;
    private float multiplier;
    List<string> duel;
    List<string> disadvantage;
    List<string> deck;
    List<string> lobby;
    List<string> lose;
    List<string> menu;
    List<string> siding;
    List<string> win;
    List<string> advantage;
    BGMType currentPlaying;
    Coroutine soundRoutine;
    Uri SoundURI;
    public static BGMController Instance;

    public enum BGMType
    {
       none = 0,
       duel = 1,
       disadvantage = 2,
       deck = 3,
       lobby = 4,
       lose = 5,
       menu = 6,
       siding = 7,
       win = 8,
       advantage = 9
    }

    public BGMController ()
    {
        currentPlaying = BGMType.none;
        BGMController.Instance = this;
        LoadAllBGM();
    }
    // Use this for initialization
    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

#if UNITY_IOS
        multiplier = 0.08f;
#else
        multiplier = 0.8f;
#endif
    }

    public void StartBGM(BGMType kind)
    {
        if (currentPlaying == kind)
            return;

        System.Random rnd = new System.Random();
        int bgmNumber = 0;
        switch (kind)
        {
            case BGMType.duel:
                bgmNumber = rnd.Next(0, duel.Count);
                PlayRandomBGM(duel[bgmNumber]);
                break;
            case BGMType.advantage:
                bgmNumber = rnd.Next(0, advantage.Count);
                PlayRandomBGM(advantage[bgmNumber]);
                break;
            case BGMType.disadvantage:
                bgmNumber = rnd.Next(0, disadvantage.Count);
                PlayRandomBGM(disadvantage[bgmNumber]);
                break;
            case BGMType.deck:
                bgmNumber = rnd.Next(0, deck.Count);
                PlayRandomBGM(deck[bgmNumber]);
                break;
            case BGMType.lobby:
                bgmNumber = rnd.Next(0, lobby.Count);
                PlayRandomBGM(lobby[bgmNumber]);
                break;
            case BGMType.lose:
                bgmNumber = rnd.Next(0, lose.Count);
                PlayRandomBGM(lose[bgmNumber]);
                break;
            case BGMType.menu:
                bgmNumber = rnd.Next(0, menu.Count);
                PlayRandomBGM(menu[bgmNumber]);
                break;
            case BGMType.siding:
                bgmNumber = rnd.Next(0, siding.Count);
                PlayRandomBGM(siding[bgmNumber]);
                break;
            case BGMType.win:
                bgmNumber = rnd.Next(0, win.Count);
                PlayRandomBGM(win[bgmNumber]);
                break;
        }

        currentPlaying = kind;
    }

    public void PlayRandomBGM(string bgmName)
    {
        SoundURI = new Uri(new Uri("file:///"), Environment.CurrentDirectory.Replace("\\", "/") + "/" + bgmName);
        soundFilePath = SoundURI.ToString();

        if (Program.I().setting != null && !Program.I().setting.isBGMMute.value)
        {
            if(soundRoutine != null)
                StopCoroutine(soundRoutine);
            soundRoutine = StartCoroutine(LoadBGM());
        }
    }

    public void LoadAllBGM()
    {
        duel = new List<string>();
        disadvantage = new List<string>();
        advantage = new List<string>();
        deck = new List<string>();
        lobby = new List<string>();
        lose = new List<string>();
        menu = new List<string>();
        siding = new List<string>();
        win = new List<string>();

        string soundPath = "sound/bgm/";
        //Unity 能使用的音频格式：.aif .wav .mp3 .ogg
        try { duel.AddRange(Directory.GetFiles(string.Concat(soundPath, "duel"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { advantage.AddRange(Directory.GetFiles(string.Concat(soundPath, "advantage"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { disadvantage.AddRange(Directory.GetFiles(string.Concat(soundPath, "disadvantage"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { deck.AddRange(Directory.GetFiles(string.Concat(soundPath, "deck"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { lobby.AddRange(Directory.GetFiles(string.Concat(soundPath, "lobby"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { lose.AddRange(Directory.GetFiles(string.Concat(soundPath, "lose"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { menu.AddRange(Directory.GetFiles(string.Concat(soundPath, "menu"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { siding.AddRange(Directory.GetFiles(string.Concat(soundPath, "siding"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
        try { win.AddRange(Directory.GetFiles(string.Concat(soundPath, "win"), "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".mp3") || s.EndsWith(".ogg")));} catch {}
    }

    public void changeBGMVol(float vol)
    {
        try
        {
            if (audioSource != null)
            {
                audioSource.volume = vol * multiplier;
            }
        }
        catch { }

    }
    private IEnumerator LoadBGM()
    {
        WWW request = GetAudioFromFile(soundFilePath);
        yield return request;
        audioClip = request.GetAudioClip(true, true);
        audioClip.name = Path.GetFileName(soundFilePath);
        PlayAudioFile();
    }

    private void PlayAudioFile()
    {
        audioSource.clip = audioClip;
        audioSource.volume = Program.I().setting.vol() * multiplier;
        audioSource.loop = true;
        audioSource.Play();
    }

    private WWW GetAudioFromFile(string pathToFile)
    {
        WWW request = new WWW(pathToFile);
        return request;
    }
}
