using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void SaveGame()
    {
        // 实现保存游戏逻辑
        // 例如使用PlayerPrefs或者文件系统保存游戏状态
    }

    public void ReturnToMainMenu()
    {
        // 实现返回主菜单逻辑
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
        bool isMenuActive = menuCanvas.activeSelf;
        menuCanvas.SetActive(!isMenuActive);
        Time.timeScale = isMenuActive ? 1 : 0;
    }

    public void OnGameVictory()
    {
        winMenu.SetActive(true);
        StartCoroutine(PlayEndAnimation());// 处理游戏胜利逻辑
    }
}