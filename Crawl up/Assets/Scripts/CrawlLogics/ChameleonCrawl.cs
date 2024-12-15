using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 引用 TextMeshPro 命名空间

public class ChameleonCrawl : MonoBehaviour
{
    public PlayerController playerController; // 通过 Inspector 手动分配
    private float timer = 0.0f;
    private float interval = 1.0f;
    private KeyCode currentKey;
    public TextMeshProUGUI letterDisplay; // 引用 TMP Text 对象

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController not assigned!");
        }

        if (letterDisplay == null)
        {
            Debug.LogError("LetterDisplay not assigned!");
        }

        ChooseRandomKey();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0.0f;
            ChooseRandomKey();
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(currentKey))
            {
                if (playerController != null)
                {
                    playerController.chameleon = true;
                }
                if (letterDisplay != null)
                {
                    letterDisplay.color = new Color(100f / 255f, 180f / 255f, 80f / 255f); // 设置颜色为 (100, 180, 80)
                }
                StartCoroutine(ResetChameleon());
            }
            else
            {
                if (playerController != null)
                {
                    playerController.chameleon = false;
                }
            }
        }
    }

    void ChooseRandomKey()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int index = Random.Range(0, letters.Length);
        currentKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), letters[index].ToString());
        if (letterDisplay != null)
        {
            letterDisplay.text = "Press the key: " + currentKey; // 更新 TMP Text 内容
        }
        Debug.Log("Press the key: " + currentKey);
    }

    IEnumerator ResetChameleon()
    {
        yield return new WaitForSeconds(0.5f);
        if (playerController != null)
        {
            playerController.chameleon = false;
        }
        if (letterDisplay != null)
        {
            letterDisplay.color = Color.white; // 恢复颜色为默认的白色
        }
    }
}