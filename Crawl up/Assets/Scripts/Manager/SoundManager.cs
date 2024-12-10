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
        for (int i = 0; i < 5; i++)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + i);
            bgmPlayer.transform.parent = this.transform;
            AudioSource audioSource = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(audioSource);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var player in BGMPlayers)
        {
            // 这里应该检查player.gameObject是否是场景中的对象
            if (player.gameObject.scene == SceneManager.GetActiveScene())
            {
                DestroyImmediate(player.gameObject, true);
            }
        }
        BGMPlayers.Clear();

        // 重新创建BGMPlayers
        for (int i = 0; i < 5; i++)
        {
            GameObject bgmPlayer = new GameObject("BGMPlayer" + i);
            bgmPlayer.transform.parent = this.transform;
            AudioSource audioSource = bgmPlayer.AddComponent<AudioSource>();
            BGMPlayers.Add(audioSource);
        }

        // 播放默认背景音乐
        PlayMusic(0, true, true, 1.0f);
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
