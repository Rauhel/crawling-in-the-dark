using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChameleonCrawl : MonoBehaviour
{
    private PlayerController playerController;
    private float timer = 0.0f;
    private float interval = 2.0f; // 将间隔时间改为 2 秒
    private float gracePeriod = 0.5f; // 允许玩家有 0.5 秒的误差
    private string currentSequence;
    private int currentIndex = 0;
    public TextMeshProUGUI letterDisplay;
    public TextMeshProUGUI nextLetterDisplay;
    private bool inputCorrect = true;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on the player.");
        }

        if (letterDisplay == null || nextLetterDisplay == null)
        {
            Debug.LogError("LetterDisplay or NextLetterDisplay not assigned!");
        }

        playerController.chameleon = true;
        GenerateRandomSequence();
    }

    void Update()
    {
        timer += Time.deltaTime;
        bool keyPressed = false;

        if (timer >= interval)
        {
            timer = 0.0f;
            UpdateSequence();
            if (!inputCorrect)
            {
                StartCoroutine(GracePeriod());
            }
            inputCorrect = true; // Reset for the next interval
        }

        if (Input.anyKeyDown)
        {
            keyPressed = true;
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), currentSequence[currentIndex].ToString())))
            {
                playerController.chameleon = true;
                letterDisplay.color = new Color(100f / 255f, 180f / 255f, 80f / 255f);
                inputCorrect = true;
            }
            else
            {
                inputCorrect = false;
            }
        }

        if (timer < interval)
        {
            inputCorrect = false;
        }
    }

    IEnumerator GracePeriod()
    {
        playerController.chameleon = true;
        yield return new WaitForSeconds(gracePeriod);
        playerController.chameleon = false;
    }

    void GenerateRandomSequence()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        currentSequence = "";
        for (int i = 0; i < 2; i++)
        {
            int index = Random.Range(0, letters.Length);
            currentSequence += letters[index];
        }
        currentIndex = 0;
        if (letterDisplay != null && nextLetterDisplay != null)
        {
            letterDisplay.text = "Press the key: " + currentSequence[0];
            nextLetterDisplay.text = "Next key: " + currentSequence[1];
            letterDisplay.color = Color.white;
        }
        Debug.Log("Press the key: " + currentSequence[0] + ", Next key: " + currentSequence[1]);
    }

    void UpdateSequence()
    {
        if (currentSequence.Length > 1)
        {
            currentSequence = currentSequence.Substring(1) + GetRandomLetter();
            if (letterDisplay != null && nextLetterDisplay != null)
            {
                letterDisplay.text = "Press the key: " + currentSequence[0];
                nextLetterDisplay.text = "Next key: " + currentSequence[1];
                letterDisplay.color = Color.white;
            }
            Debug.Log("Press the key: " + currentSequence[0] + ", Next key: " + currentSequence[1]);
        }
    }

    char GetRandomLetter()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int index = Random.Range(0, letters.Length);
        return letters[index];
    }
}