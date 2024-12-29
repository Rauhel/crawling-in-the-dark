using UnityEngine;

[RequireComponent(typeof(NpcPatrol))]
public class NpcVisualFeedback : MonoBehaviour
{
    [Header("检测范围指示器")]
    public GameObject rangeIndicatorPrefab;  // 在Inspector中指定范围指示器的预制体
    private SpriteRenderer rangeIndicator;   // 范围指示器的SpriteRenderer

    [Header("颜色设置")]
    public Color hostileColor = new Color(1f, 0.3f, 0.3f, 0.5f);    // 追击状态的红色（半透明）
    public Color friendlyColor = new Color(0.3f, 1f, 0.3f, 0.5f);   // 对话状态的绿色（半透明）
    public Color neutralColor = new Color(1f, 1f, 1f, 0.5f);        // 中性状态的白色（半透明）
    public Color lethalColor = new Color(0.7f, 0f, 1f, 0.5f);       // 致命状态的紫色（半透明）

    private NpcPatrol npcPatrol;
    private PlayerInput playerInput;

    private void Start()
    {
        npcPatrol = GetComponent<NpcPatrol>();
        playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();

        // 创建检测范围指示器
        if (rangeIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            indicator.transform.parent = transform;
            rangeIndicator = indicator.GetComponent<SpriteRenderer>();
            
            // 设置范围指示器的大小以匹配椭圆检测范围
            float npcScale = transform.localScale.x;  // 假设x和y的缩放是一样的
            float horizontalScale = npcPatrol.horizontalDetectionRange * 2 / npcScale;
            float verticalScale = npcPatrol.verticalDetectionRange * 2 / npcScale;
            indicator.transform.localScale = new Vector3(horizontalScale, verticalScale, 1);
            
            // 确保范围指示器在NPC精灵的后面
            rangeIndicator.sortingOrder = -1;
        }
    }

    private void Update()
    {
        if (playerInput == null || rangeIndicator == null) return;
        UpdateVisualFeedback();
    }

    private void UpdateVisualFeedback()
    {
        string currentCrawlType = playerInput.currentCrawlName;
        bool isHostileCrawl = IsHostileCrawlType(currentCrawlType);
        bool canTriggerDialogue = CanTriggerDialogue(currentCrawlType);
        bool isLethalCrawl = IsLethalCrawlType(currentCrawlType);

        Color targetColor = neutralColor;
        if (isLethalCrawl)
        {
            targetColor = lethalColor;
        }
        else if (isHostileCrawl)
        {
            targetColor = hostileColor;
        }
        else if (canTriggerDialogue)
        {
            targetColor = friendlyColor;
        }

        rangeIndicator.color = targetColor;
    }

    private bool IsHostileCrawlType(string crawlType)
    {
        if (string.IsNullOrEmpty(crawlType) || npcPatrol.detectionSettings == null) 
            return false;

        // 检查是否是敌对爬行类型
        if (System.Enum.TryParse<CrawlType>(crawlType, true, out CrawlType currentCrawlType))
        {
            return System.Array.Exists(npcPatrol.detectionSettings.hostileCrawlTypes, 
                type => type == currentCrawlType);
        }
        return false;
    }

    private bool CanTriggerDialogue(string crawlType)
    {
        if (string.IsNullOrEmpty(crawlType) || npcPatrol.dialogueTriggers == null) 
            return false;

        // 检查是否有可触发的对话
        if (System.Enum.TryParse<CrawlType>(crawlType, true, out CrawlType currentCrawlType))
        {
            return System.Array.Exists(npcPatrol.dialogueTriggers, 
                trigger => trigger.requiredCrawlType == currentCrawlType && !trigger.hasTriggered);
        }
        return false;
    }

    private bool IsLethalCrawlType(string crawlType)
    {
        if (string.IsNullOrEmpty(crawlType) || npcPatrol.detectionSettings == null) 
            return false;

        // 检查是否是致命爬行类型
        if (System.Enum.TryParse<CrawlType>(crawlType, true, out CrawlType currentCrawlType))
        {
            return System.Array.Exists(npcPatrol.detectionSettings.lethalCrawlTypes, 
                type => type == currentCrawlType);
        }
        return false;
    }

    private void OnDestroy()
    {
        // 范围指示器会随着父对象自动销毁，不需要手动处理
    }
} 