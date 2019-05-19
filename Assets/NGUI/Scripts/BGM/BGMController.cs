using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BGMController : MonoBehaviour
{
    public string soundFilePath;
    public string soundExtension;
    public AudioSource audioSource;
    AudioClip audioClip;
    private float multiplier;
    List<string> btl_general;
    List<string> btl_losing;
    List<string> deck;
    List<string> lobby;
    List<string> lose;
    List<string> menu;
    List<string> siding;
    List<string> win;
    List<string> btl_winning;
    BGMType currentPlaying;
    Coroutine soundRoutine;
    Uri SoundURI;
    public static BGMController Instance;

    public enum BGMType
    {
       none = 0,
       btl_general = 1,
       btl_losing = 2,
       deck = 3,
       lobby = 4,
       lose = 5,
       menu = 6,
       siding = 7,
       win = 8,
       btl_winning = 9
    }

    public BGMController ()
    {
        currentPlaying = BGMType.none;
        soundExtension = ".ogg";
        BGMController.Instance = this;
        LoadAllBGM();
    }
    // Use this for initialization
    public void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        
        
#if UNITY_IOS
        multiplier=0.08f;
#endif
        multiplier = 0.8f;
    }

    public void StartBGM(BGMType kind)
    {
        if (currentPlaying == kind)
            return;

        string bgmFiles = "sound/bgm";
        System.Random rnd = new System.Random();
        int bgmNumber = 1;
        switch (kind)
        {
            case BGMType.btl_general:
                bgmFiles += "/btl_general";
                bgmNumber = rnd.Next(1, btl_general.Count);
                break;
            case BGMType.btl_winning:
                bgmFiles += "/btl_winning";
                bgmNumber = rnd.Next(1, btl_winning.Count);
                break;
            case BGMType.btl_losing:
                bgmFiles += "/btl_losing";
                bgmNumber = rnd.Next(1, btl_losing.Count);
                break;
            case BGMType.deck:
                bgmFiles += "/deck";
                bgmNumber = rnd.Next(1, deck.Count);
                break;
            case BGMType.lobby:
                bgmFiles += "/lobby";
                bgmNumber = rnd.Next(1, lobby.Count);
                break;
            case BGMType.lose:
                bgmFiles += "/lose";
                bgmNumber = rnd.Next(1, lose.Count);
                break;
            case BGMType.menu:
                bgmFiles += "/menu";
                bgmNumber = rnd.Next(1, menu.Count);
                break;
            case BGMType.siding:
                bgmFiles += "/siding";
                bgmNumber = rnd.Next(1, siding.Count);
                break;
            case BGMType.win:
                bgmFiles += "/win";
                bgmNumber = rnd.Next(1, win.Count);
                break;
        }

        PlayRandomBGM(bgmFiles, bgmNumber);
        currentPlaying = kind;
    }

    public void PlayRandomBGM(string bgmPath, int bgmNumber)
    {
        SoundURI = new Uri(new Uri("file:///"), Environment.CurrentDirectory.Replace("\\", "/") + "/" + bgmPath + "/" + bgmNumber.ToString() + soundExtension);
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
        btl_general = new List<string>();
        btl_losing = new List<string>();
        btl_winning = new List<string>();
        deck = new List<string>();
        lobby = new List<string>();
        lose = new List<string>();
        menu = new List<string>();
        siding = new List<string>();
        win = new List<string>();

        string soundPath = "sound/bgm/";
        btl_general.AddRange(Directory.GetFiles(string.Concat(soundPath, "btl_general"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        btl_winning.AddRange(Directory.GetFiles(string.Concat(soundPath, "btl_winning"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        btl_losing.AddRange(Directory.GetFiles(string.Concat(soundPath, "btl_losing"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        deck.AddRange(Directory.GetFiles(string.Concat(soundPath, "deck"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        lobby.AddRange(Directory.GetFiles(string.Concat(soundPath, "lobby"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        lose.AddRange(Directory.GetFiles(string.Concat(soundPath, "lose"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        menu.AddRange(Directory.GetFiles(string.Concat(soundPath, "menu"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        siding.AddRange(Directory.GetFiles(string.Concat(soundPath, "siding"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
        win.AddRange(Directory.GetFiles(string.Concat(soundPath, "win"), "*" + soundExtension, SearchOption.TopDirectoryOnly));
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
