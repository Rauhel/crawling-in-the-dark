using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject winMenu;
    public Animator endAnimator;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void PlayerWin()
    {
        winMenu.SetActive(true);
        StartCoroutine(PlayEndAnimation());
    }

    private IEnumerator PlayEndAnimation()
    {
        endAnimator.SetTrigger("PlayEnd");
        yield return new WaitForSeconds(endAnimator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene("MainMenu");
    }
}