using UnityEngine;
using TMPro;
using System.Collections;

public class PromptManager : MonoBehaviour
{
    public static PromptManager Instance { get; private set; }

    [Header("提示UI")]
    public GameObject interactionPrompt;  // 交互提示（按Q交互）
    public GameObject basicMoveTutorial;  // 基础移动教程
    public GameObject treeTutorial;       // 树根穿过提示
    public TextMeshProUGUI crawlTypeText; // 当前爬行方式提示
    public TextMeshProUGUI fragmentProgressText; // 碎片进度提示
    private TextMeshProUGUI treePromptText;  // 树根提示文本组件

    private bool hasShownBasicMoveTutorial = false;
    private PlayerInput playerInput;
    private bool canShowTreePrompt = true;  // 是否可以显示树根提示
    private Coroutine treePromptCoroutine;  // 存储协程引用

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
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

        // 获取玩家输入组件
        playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();
        
        // 初始化爬行方式文本
        if (crawlTypeText && playerInput != null)
        {
            crawlTypeText.gameObject.SetActive(true);
            UpdateCrawlTypeText();
        }

        // 初始化碎片进度文本
        InitializeFragmentProgress();
    }

    private void InitializeFragmentProgress()
    {
        if (fragmentProgressText != null)
        {
            fragmentProgressText.gameObject.SetActive(true);
            UpdateFragmentProgress();
            Debug.Log("初始化碎片进度显示");
        }
        else
        {
            Debug.LogError("fragmentProgressText 未设置！请在 Unity Inspector 中设置引用。");
        }
    }

    public void UpdateFragmentProgress()
    {
        if (fragmentProgressText != null && PlayerManager.Instance != null)
        {
            int fragmentCount = PlayerManager.Instance.GetFragmentCount();
            int totalFragments = 7; // 从 PlayerManager 中的常量获取
            fragmentProgressText.text = $"收集碎片：{fragmentCount}/{totalFragments}";
            Debug.Log($"更新碎片进度：{fragmentCount}/{totalFragments}");
        }
        else
        {
            Debug.LogError("无法更新碎片进度：fragmentProgressText 或 PlayerManager 未初始化");
        }
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

        // 更新爬行方式文本
        if (crawlTypeText && playerInput != null)
        {
            UpdateCrawlTypeText();
        }
    }

    private void UpdateCrawlTypeText()
    {
        string currentCrawlType = playerInput.currentCrawlName;
        string displayName = GetCrawlTypeDisplayName(currentCrawlType);
        crawlTypeText.text = $"当前爬行方式：{displayName}";
    }

    private string GetCrawlTypeDisplayName(string crawlType)
    {
        switch (crawlType)
        {
            case "Basic": return "基础爬行";
            case "Gecko": return "壁虎爬行";
            case "Turtle": return "乌龟爬行";
            case "Snake": return "蛇行";
            case "Cat": return "猫爬行";
            case "Chameleon": return "变色龙爬行";
            default: return crawlType;
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
        if (treeTutorial && canShowTreePrompt)
        {
            canShowTreePrompt = false;  // 防止重复触发
            if (treePromptCoroutine != null)
            {
                StopCoroutine(treePromptCoroutine);
            }
            treePromptCoroutine = StartCoroutine(ShowTreePromptDelayed());
        }
    }

    private IEnumerator ShowTreePromptDelayed()
    {
        yield return new WaitForSeconds(1f);  // 延迟1秒显示
        treeTutorial.SetActive(true);
        Debug.Log("显示树根穿过提示");
        yield return new WaitForSeconds(1f);  // 显示1秒
        HideTreePrompt();
    }

    public void HideTreePrompt()
    {
        if (treeTutorial)
        {
            treeTutorial.SetActive(false);
            Debug.Log("隐藏树根穿过提示");
        }
    }

    public void ResetTreePrompt()
    {
        canShowTreePrompt = true;  // 重置状态，允许再次显示
        if (treePromptCoroutine != null)
        {
            StopCoroutine(treePromptCoroutine);
            treePromptCoroutine = null;
        }
        HideTreePrompt();
    }
} 