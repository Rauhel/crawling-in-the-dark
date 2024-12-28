using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    [Header("提示UI")]
    public GameObject interactionPrompt;  // 交互提示（按Q交互）
    public GameObject basicMoveTutorial;  // 基础移动教程
    public GameObject treeTutorial;       // 树根穿过提示
    private TextMeshProUGUI treePromptText;  // 树根提示文本组件

    private bool hasShownBasicMoveTutorial = false;

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

        // 获取树根提示的文本组件
        if (treeTutorial)
        {
            treePromptText = treeTutorial.GetComponent<TextMeshProUGUI>();
            if (treePromptText)
            {
                treePromptText.text = "按P穿过根部";
            }
        }
    }

    private void Start()
    {
        // 确保所有提示一开始都是隐藏的
        if (interactionPrompt) interactionPrompt.SetActive(false);
        if (basicMoveTutorial) basicMoveTutorial.SetActive(false);
        if (treeTutorial) treeTutorial.SetActive(false);
        
        // 显示基础移动教程
        ShowBasicMoveTutorial();
    }

    private void Update()
    {
        // 检查是否需要隐藏基础移动教程
        if (hasShownBasicMoveTutorial && basicMoveTutorial && basicMoveTutorial.activeSelf)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                basicMoveTutorial.SetActive(false);
                Debug.Log("隐藏基础移动教程");
            }
        }
    }

    public void ShowInteractionPrompt()
    {
        if (interactionPrompt)
        {
            interactionPrompt.SetActive(true);
            Debug.Log("显示交互提示");
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt)
        {
            interactionPrompt.SetActive(false);
            Debug.Log("隐藏交互提示");
        }
    }

    private void ShowBasicMoveTutorial()
    {
        if (basicMoveTutorial && !hasShownBasicMoveTutorial)
        {
            basicMoveTutorial.SetActive(true);
            hasShownBasicMoveTutorial = true;
            Debug.Log("显示基础移动教程");
        }
    }

    public void ShowTreePrompt()
    {
        if (treeTutorial)
        {
            treeTutorial.SetActive(true);
            Debug.Log("显示树根穿过提示");
        }
    }

    public void HideTreePrompt()
    {
        if (treeTutorial)
        {
            treeTutorial.SetActive(false);
            Debug.Log("隐藏树根穿过提示");
        }
    }
} 