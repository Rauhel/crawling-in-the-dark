using UnityEngine;

public class NpcVisualFeedback : MonoBehaviour
{
    public GameObject rangeIndicatorPrefab;
    private GameObject indicator;
    private NpcPatrol npcPatrol;
    private PlayerInput playerInput;
    private SpriteRenderer indicatorRenderer;

    [Header("颜色设置")]
    public Color hostileColor = new Color(1f, 0.3f, 0.3f, 0.5f);    // 追击状态的红色（半透明）
    public Color friendlyColor = new Color(0.3f, 1f, 0.3f, 0.5f);   // 对话状态的绿色（半透明）
    public Color neutralColor = new Color(1f, 1f, 1f, 0.5f);        // 中性状态的白色（半透明）
    public Color lethalColor = new Color(0.7f, 0f, 1f, 0.5f);       // 致命状态的紫色（半透明）

    void Start()
    {
        npcPatrol = GetComponent<NpcPatrol>();
        playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();
        
        if (npcPatrol != null && rangeIndicatorPrefab != null)
        {
            indicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            indicator.transform.SetParent(transform);
            
            // 获取并设置指示器的SpriteRenderer
            indicatorRenderer = indicator.GetComponent<SpriteRenderer>();
            if (indicatorRenderer != null)
            {
                // 获取NPC的SpriteRenderer的sortingOrder并将指示器设置为比它小1
                SpriteRenderer npcRenderer = GetComponent<SpriteRenderer>();
                if (npcRenderer != null)
                {
                    indicatorRenderer.sortingOrder = npcRenderer.sortingOrder - 1;
                }
            }
            
            UpdateRangeIndicator();
        }
    }

    void Update()
    {
        UpdateRangeIndicator();
        UpdateVisualFeedback();
    }

    void UpdateRangeIndicator()
    {
        if (indicator != null && npcPatrol != null)
        {
            // 获取NPC的朝向和偏移量
            float actualOffset = transform.localScale.x < 0 ? npcPatrol.horizontalOffset : -npcPatrol.horizontalOffset;
            Vector3 offsetPosition = transform.position + new Vector3(actualOffset, 0, 0);
            
            // 更新指示器的位置
            indicator.transform.position = offsetPosition;
            
            // 考虑NPC的缩放比例来计算指示器大小
            float npcScale = Mathf.Abs(transform.localScale.x);  // 假设x和y的缩放是一样的
            float horizontalScale = npcPatrol.horizontalDetectionRange * 2 / npcScale;
            float verticalScale = npcPatrol.verticalDetectionRange * 2 / npcScale;
            indicator.transform.localScale = new Vector3(horizontalScale, verticalScale, 1);
        }
    }

    void UpdateVisualFeedback()
    {
        if (playerInput == null || indicatorRenderer == null) return;

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

        indicatorRenderer.color = targetColor;
    }

    private bool IsHostileCrawlType(string crawlType)
    {
        if (string.IsNullOrEmpty(crawlType) || npcPatrol.detectionSettings == null) 
            return false;

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

        if (System.Enum.TryParse<CrawlType>(crawlType, true, out CrawlType currentCrawlType))
        {
            return System.Array.Exists(npcPatrol.detectionSettings.lethalCrawlTypes, 
                type => type == currentCrawlType);
        }
        return false;
    }

    void OnDestroy()
    {
        // 范围指示器会随着父对象自动销毁，不需要手动处理
    }
} 