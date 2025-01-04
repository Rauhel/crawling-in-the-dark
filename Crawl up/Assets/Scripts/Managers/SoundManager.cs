using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(SoundManager).ToString());
                    instance = singleton.AddComponent<SoundManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 初始化音频播放器
            InitializeBGMPlayers();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public List<AudioSource> BGMPlayers = new List<AudioSource>();
    public List<AudioClip> MyMusicList;
    public List<AudioClip> MySFXList;

    private void Start()
    {
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // 播放默认背景音乐
        PlayMusic(0, true, true, 1.0f);
    }

    private void OnDestroy()
    {
        // 取消订阅场景加载事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeBGMPlayers()
    {
        // 清理现有的播放器
        foreach (var player in BGMPlayers)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
        BGMPlayers.Clear();

        // 创建新的播放器
        for (int i = 0; i < 5; i++)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + i);
            bgmPlayer.transform.parent = this.transform;
            AudioSource audioSource = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(audioSource);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 停止所有当前播放的音乐
        foreach (var player in BGMPlayers)
        {
            if (player != null && player.isPlaying)
            {
                player.Stop();
            }
        }

        // 重新初始化音频播放器
        InitializeBGMPlayers();

        // 根据场景名称播放对应的背景音乐
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(0, true, true, 1.0f); // 播放主菜单音乐
                break;
            case "GameScene":
                PlayMusic(1, true, true, 1.0f); // 播放游戏场景音乐
                break;
            default:
                PlayMusic(0, true, true, 1.0f); // 默认音乐
                break;
        }
        
        Debug.Log($"场景 {scene.name} 加载完成，开始播放背景音乐");
    }

    public void PlayMusic(int index, bool isPlay, bool isLoop, float volume = 1.0f)
    {
        AudioSource player = BGMPlayers.Find(p => p.clip == MyMusicList[index] && p.isPlaying == isPlay);
        if (player == null)
        {
            player = BGMPlayers.Find(p => p.isPlaying == false);
            if (player == null)
            {
                GameObject bgmPlayer = new GameObject("BGMPlayer" + BGMPlayers.Count);
                bgmPlayer.transform.parent = this.transform;
                player = bgmPlayer.AddComponent<AudioSource>();
                BGMPlayers.Add(player);
            }
            player.clip = MyMusicList[index];
        }
        player.loop = isLoop;
        player.volume = volume;
        if (isPlay)
        {
            player.Play();
        }
        else
        {
            player.Stop();
        }
    }

    public void PlayOneShotMusic(int index, float volume = 1.0f)
    {
        AudioSource player = BGMPlayers.Find(p => p.isPlaying == false);
        if (player == null)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + BGMPlayers.Count);
            bgmPlayer.transform.parent = this.transform;
            player = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(player);
        }
        player.clip = MyMusicList[index];
        player.volume = volume;
        player.PlayOneShot(player.clip);
    }

    public void PlaySFX(int index, bool isPlay, bool isLoop, float volume = 1.0f)
    {
        AudioSource player = BGMPlayers.Find(p => p.clip == MySFXList[index] && p.isPlaying == false);
        if (player == null)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + BGMPlayers.Count);
            bgmPlayer.transform.parent = this.transform;
            player = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(player);
        }
        player.clip = MySFXList[index];
        player.loop = isLoop;
        player.volume = volume;
        if (isPlay)
        {
            player.Play();
        }
        else
        {
            player.Stop();
        }
    }

    public void PlayOneShotSFX(int index, float volume = 1.0f)
    {
        AudioSource player = BGMPlayers.Find(p => p.isPlaying == false);
        if (player == null)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + BGMPlayers.Count);
            bgmPlayer.transform.parent = this.transform;
            player = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(player);
        }
        player.clip = MySFXList[index];
        player.volume = volume;
        // PlayOneShot不需要音量参数，音量通过AudioSource.volume设置
        player.PlayOneShot(player.clip);
    }
}
