using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class m_GameManager : MonoBehaviour
{
    public static m_GameManager Instance { get; private set; }

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

    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject menuCanvas;
    public Animator endAnimator;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        // 确保所有UI面板在开始时都是禁用的
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        if (winMenu != null)
            winMenu.SetActive(false);
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
        
        // 重置暂停状态
        isPaused = false;
        Time.timeScale = 1;
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
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator PlayEndAnimation()
    {
        endAnimator.SetTrigger("PlayEnd");
        yield return new WaitForSeconds(endAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
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
        winMenu.SetActive(true);
        StartCoroutine(PlayEndAnimation());// 处理游戏胜利逻辑
    }

    // 添加重生功能
    public void RespawnPlayer()
    {
        // 触发玩家死亡事件
        EventCenter.Instance.Publish(EventCenter.EVENT_PLAYER_DIED);
        
        // 恢复游戏
        ResumeGame();
    }
}