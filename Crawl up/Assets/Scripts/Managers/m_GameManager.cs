using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class m_GameManager : MonoBehaviour
{
    public static m_GameManager Instance { get; private set; }

    [Header("UI设置")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject menuCanvas;
    public Image fadeImage; // 用于淡入淡出的UI图片
    public float fadeDuration = 1.5f; // 淡入淡出持续时间，增加到1.5秒
    public float blackScreenDuration = 1f; // 黑屏持续时间，设为1秒

    [Header("背景设置")]
    public SpriteRenderer backgroundRenderer;

    [Header("玩家设置")]
    private SpriteRenderer playerRenderer; // 玩家的SpriteRenderer
    private bool isRespawning = false;

    private bool isPaused = false;
    private bool isGameVictory = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 确保所有UI面板在开始时都是禁用的
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (winMenu != null)
            winMenu.SetActive(false);
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
        
        // 获取玩家的SpriteRenderer
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRenderer = player.GetComponent<SpriteRenderer>();
        }

        // 初始化淡入淡出图片
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0); // 开始时完全透明
        }
        
        // 重置暂停状态
        isPaused = false;
        Time.timeScale = 1;

        // 订阅玩家死亡事件
        EventCenter.Instance.Subscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    void OnDestroy()
    {
        // 取消订阅事件
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    // 处理玩家死亡事件
    private void OnPlayerDied()
    {
        StartCoroutine(ReviveSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                TogglePause();
            }
        }

        if (isGameVictory && Input.anyKeyDown)
        {
            ReturnToMainMenu();
        }
    }

    public void TogglePause()
    {
        // 如果正在对话，不允许暂停
        if (DialogueManager.Instance.IsDialogueActive())
            return;

        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ReturnToMainMenu()
    {
        // 保存游戏状态
        PlayerManager.Instance.SaveGameState();
        Time.timeScale = 1; // 确保返回主菜单时时间恢复正常
        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        // 简单地恢复游戏，不需要黑屏效果
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReviveAtCheckpoint()
    {
        // 触发玩家死亡事件
        EventCenter.Instance.Publish(EventCenter.EVENT_PLAYER_DIED);
    }

    private IEnumerator ReviveSequence()
    {
        // 关闭暂停菜单并恢复时间流动（因为需要协程工作）
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        // 等待一帧确保UI状态更新
        yield return null;

        // 开始重生过程
        yield return StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        if (isRespawning) yield break;
        
        isRespawning = true;

        // 淡出效果
        yield return StartCoroutine(FadeEffect(true));

        // 重置玩家位置到最后的安全点
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.transform.position = PlayerManager.Instance.GetLastSafePoint();
        }

        // 增加黑屏等待时间
        yield return new WaitForSeconds(blackScreenDuration);

        // 淡入效果
        yield return StartCoroutine(FadeEffect(false));

        // 确保玩家完全可见
        if (playerRenderer != null)
        {
            Color color = playerRenderer.color;
            playerRenderer.color = new Color(color.r, color.g, color.b, 1f);
        }

        isRespawning = false;
    }

    public void ToggleMenu()
    {
        // 如果正在对话，不允许打开菜单
        if (DialogueManager.Instance.IsDialogueActive())
            return;

        bool isMenuActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isMenuActive);
        Time.timeScale = isMenuActive ? 1 : 0;
    }

    public void OnGameVictory()
    {
        Debug.Log("游戏胜利！");
        isGameVictory = true;
        Time.timeScale = 1; // 确保胜利时时间恢复正常
        
        // 播放胜利音效
        SoundManager.Instance.PlaySFX(6, true, false, 1f);
        
        // 显示胜利菜单
        if (winMenu != null)
        {
            winMenu.SetActive(true);
        }
    }

    private IEnumerator FadeEffect(bool fadeOut)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeOut ? 
                elapsedTime / fadeDuration : 
                1 - (elapsedTime / fadeDuration);

            // 更新UI图片的透明度
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, alpha);
            }

            // 更新玩家精灵的透明度
            if (playerRenderer != null)
            {
                Color playerColor = playerRenderer.color;
                playerRenderer.color = new Color(playerColor.r, playerColor.g, playerColor.b, alpha);
            }

            yield return null;
        }

        // 确保淡入淡出效果完全完成
        if (fadeOut)
        {
            if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 1);
            if (playerRenderer != null) playerRenderer.color = new Color(playerRenderer.color.r, playerRenderer.color.g, playerRenderer.color.b, 0);
        }
        else
        {
            if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
            if (playerRenderer != null) playerRenderer.color = new Color(playerRenderer.color.r, playerRenderer.color.g, playerRenderer.color.b, 1);
        }
    }

    // 添加背景切换方法
    public void ChangeBackground(Sprite newBackground)
    {
        if (backgroundRenderer != null && newBackground != null)
        {
            backgroundRenderer.sprite = newBackground;
        }
    }
}