using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    [Header("提示UI引用")]
    public GameObject interactionPrompt;  // 交互提示（按Q交互）
    public GameObject crawlPrompt;        // 爬行提示
    public GameObject tutorialPrompt;     // 教程提示

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PromptManager实例已创建");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideAllPrompts();
    }

    public void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            Debug.Log("显示交互提示");
        }
        else
        {
            Debug.LogWarning("交互提示UI未设置");
        }
    }

    public void HideInteractionPrompt(string reason = "未指定原因")
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
            Debug.Log($"隐藏交互提示，原因: {reason}");
        }
    }

    public void ShowCrawlPrompt(string message)
    {
        if (crawlPrompt != null)
        {
            TextMeshProUGUI textComponent = crawlPrompt.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
                crawlPrompt.SetActive(true);
                Debug.Log($"显示爬行提示: {message}");
            }
        }
    }

    public void HideCrawlPrompt()
    {
        if (crawlPrompt != null)
        {
            crawlPrompt.SetActive(false);
        }
    }

    public void ShowTutorialPrompt(string message)
    {
        if (tutorialPrompt != null)
        {
            TextMeshProUGUI textComponent = tutorialPrompt.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
                tutorialPrompt.SetActive(true);
                Debug.Log($"显示教程提示: {message}");
            }
        }
    }

    public void HideTutorialPrompt()
    {
        if (tutorialPrompt != null)
        {
            tutorialPrompt.SetActive(false);
        }
    }

    public void HideAllPrompts()
    {
        HideInteractionPrompt();
        HideCrawlPrompt();
        HideTutorialPrompt();
    }
} 