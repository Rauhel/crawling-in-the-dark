using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    [Header("爬行设置")]
    public CrawlSettings basicCrawl;
    public CrawlSettings geckoCrawl;
    public CrawlSettings turtleCrawl;
    public CrawlSettings snakeCrawl;
    public CrawlSettings catCrawl;
    public CrawlSettings chameleonCrawl;

    [Header("动画设置")]
    private SpriteRenderer spriteRenderer;  // 用于显示精灵
    private float animationTimer = 0f;  // 动画计时器

    // 注释掉 Animator 引用
    // public Animator animator;
    public float moveDistance = 1f;

    public int currentKeyIndex = 0;
    public CrawlSettings currentCrawlSettings;
    public string currentCrawlName { get; private set; }  // 允许外部读取但只能在内部修改
    private bool isReversing = false;
    public List<KeyCode> currentKeys = new List<KeyCode>();
    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
    private bool keyDetected = false;

    // 添加动画关键帧相关变量
    private int currentAnimFrame = 0;
    public int totalAnimFrames = 4; // 动画总关键帧数，根据实际动画设置

    // 在现有变量后添加
    private Dictionary<string, CrawlProgress> crawlProgresses = new Dictionary<string, CrawlProgress>();
    public float maxTimeBetweenInputs = 2f;  // 输入序列的最大间隔时间
    
    // 添加新的字段
    private Vector2 currentMoveDirection = Vector2.right;  // 默认向右移动
    private ContactPoint2D currentContact;  // 存储当前的接触点信息
    private bool isInContact = false;  // 是否正在接触表面
    public bool canCrawlOnCurrentSurface = false;  // 是否可以在当前表面爬行
    private Rigidbody2D rb;

    // 添加新字段来跟踪实际速度
    [SerializeField] private float currentActualSpeed = 0f;
    
    // 速度计算相关变量
    public float speedCalculationWindow = 1f;  // 速度计算的时间窗口
    private Vector2 lastPosition;              // 上一次记录的位置
    private float timeSinceLastSpeedCalc = 0f; // 距离上次速度计算的时间
    
    // 添加公共属性供其他脚本访问当前实际速度
    public float CurrentActualSpeed => currentActualSpeed;

    void Start()
    {
        currentKeyIndex = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();  // 获取SpriteRenderer组件
        lastPosition = transform.position;  // 初始化位置记录

        // 使用CrawlSettingsInitializer初始化所有爬行设置
        CrawlSettingsInitializer.InitializeSettings(
            ref basicCrawl,
            ref geckoCrawl,
            ref turtleCrawl,
            ref snakeCrawl,
            ref catCrawl,
            ref chameleonCrawl
        );

        // 初始化进度字典
        foreach (var crawlType in new[] { "Basic", "Gecko", "Turtle", "Snake", "Cat", "Chameleon" })
        {
            crawlProgresses[crawlType] = new CrawlProgress();
        }

        rb = GetComponent<Rigidbody2D>();
        
        // 最后设置当前爬行类型
        ChangeCrawlType("Basic");
    }

    void Update()
    {
        // 更新速度计算
        timeSinceLastSpeedCalc += Time.deltaTime;
        if (timeSinceLastSpeedCalc >= speedCalculationWindow)
        {
            Vector2 currentPosition = transform.position;
            float distance = Vector2.Distance(currentPosition, lastPosition);
            currentActualSpeed = distance / speedCalculationWindow;
            
            lastPosition = currentPosition;
            timeSinceLastSpeedCalc = 0f;
        }

        currentKeys.Clear();
        
        // 改用 Input.anyKey 来检测按键的持续状态
        if (Input.anyKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                {
                    currentKeys.Add(key);
                }
            }
            if (currentKeys.Count > 0)
            {
                Debug.Log("Current keys: " + string.Join(", ", currentKeys));
            }
        }
        
        CheckInput();
        CheckParallelInput();
        CheckSpaceInput();        
        // 移动后根据当前接触点重新调整旋转
        if (currentContact.collider != null)
        {
            AdjustRotationToSurface(currentContact);
        }
    }

    void CheckSpaceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isReversing = !isReversing;
            // 当切换方向时重置所有进度
            ResetAllProgress();
            currentKeyIndex = 0;
            
            // 立即更新精灵朝向
            Vector3 scale = transform.localScale;
            scale.x = isReversing ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
            
            Debug.Log($"方向切换: {(isReversing ? "反向" : "正向")}");
        }
    }

    void CheckInput()
    {
        foreach (var crawlType in new[] { "Basic", "Gecko", "Turtle", "Snake", "Cat", "Chameleon" })
        {
            CrawlSettings crawlSettings = (CrawlSettings)GetType().GetField(crawlType.ToLower() + "Crawl", BindingFlags.Public | BindingFlags.Instance).GetValue(this);
            if (!crawlSettings.canCrawl || crawlSettings.keyLists == null || crawlSettings.keyLists.Length == 0) 
                continue;

            // 如果不是当前爬行方式，检查是否需要切换
            if (crawlType != currentCrawlName)
            {
                KeyList firstKeyList = crawlSettings.keyLists[0];
                if (IsValidKeyListInput(firstKeyList))
                {
                    Debug.Log($"切换到 {crawlType} 爬行方式");
                    ChangeCrawlType(crawlType);
                    return; // 切换后立即返回，这一次不移动
                }
            }
        }
    }

    private bool IsValidKeyListInput(KeyList keyList)
    {
        // 检查是否存在无效按键
        foreach (KeyCode pressedKey in currentKeys)
        {
            if (!keyList.keySequence.Contains(pressedKey))
            {
                return false;
            }
        }

        // 计算有效按键数量
        int keyCount = keyList.keySequence.Count(key => currentKeys.Contains(key));
        
        // 检查是否完全匹配
        if (keyCount != currentKeys.Count)
        {
            return false;
        }
        
        // 检查按键数是否在允许的范围内
        bool isValid = keyCount >= keyList.requiredMinKeyCount && keyCount <= keyList.requiredMaxKeyCount;
        return isValid;
    }

    void MovePlayer()
    {
        if (!canCrawlOnCurrentSurface)
        {
            Debug.Log("无法在当前表面爬行");
            return;
        }
        
        Debug.Log("Moving player...");
        float speed = GetSpeedBasedOnSurface();
        Vector2 moveDirection = isReversing ? -currentMoveDirection : currentMoveDirection;

        // 使用Rigidbody2D移动一个固定距离
        Vector2 targetPosition = rb.position + (moveDirection * moveDistance);
        rb.MovePosition(targetPosition);

        // 播放移动音效，参数分别是：音效索引，是否播放，是否循环，音量
        SoundManager.Instance.PlaySFX(0, true, false, 1f);

        // 移动后重新调整旋转
        if (currentContact.collider != null)
        {
            AdjustRotationToSurface(currentContact);
        }
    }

    float GetSpeedBasedOnSurface()
    {
        if (currentContact.collider != null)
        {
            string surfaceTag = currentContact.collider.tag;
            switch (surfaceTag)
            {
                case "Ground":
                    return currentCrawlSettings.groundSpeed;
                case "Water":
                    return currentCrawlSettings.waterSpeed;
                case "Ice":
                    return currentCrawlSettings.iceSpeed;
                case "Tree":
                    return currentCrawlSettings.treeSpeed;
                case "Slope":
                    return currentCrawlSettings.slopeSpeed;
                default:
                    return currentCrawlSettings.groundSpeed;
            }
        }
        return currentCrawlSettings.groundSpeed;  // 默认返回地面速度
    }

    void PlayAnimation(string crawlName)
    {
        // 注释掉动画触发
        // animator.SetTrigger(crawlName);
    }

    // Call this method to change the current crawl settings
    public void ChangeCrawlType(string crawlType)
    {
        // 先把所有爬行方式的isActive设为false
        basicCrawl.isActive = false;
        geckoCrawl.isActive = false;
        turtleCrawl.isActive = false;
        snakeCrawl.isActive = false;
        catCrawl.isActive = false;
        chameleonCrawl.isActive = false;

        FieldInfo field = GetType().GetField(crawlType.ToLower() + "Crawl", BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            currentCrawlSettings = (CrawlSettings)field.GetValue(this);
            currentCrawlName = crawlType;

            // 只设置当前爬行方式为active
            currentCrawlSettings.isActive = true;
            Debug.Log($"切换到 {crawlType} 爬行方式");

            // 切换背景
            if (currentCrawlSettings.backgroundSprite != null)
            {
                m_GameManager.Instance.ChangeBackground(currentCrawlSettings.backgroundSprite);
            }

            // 如果当前正在接触表面，检查爬行状态
            if (isInContact)
            {
                CheckCrawlability();
            }
        }
        ResetAllProgress();
    }

    // 添加新的辅助方法
    private void ResetProgress(string crawlType)
    {
        var progress = crawlProgresses[crawlType];
        progress.currentKeyListIndex = 0;
        progress.isInProgress = false;
        progress.lastValidInputTime = 0f;
    }

    private void ResetAllProgress()
    {
        foreach (var crawlType in crawlProgresses.Keys.ToList())
        {
            ResetProgress(crawlType);
        }
    }

    void CheckParallelInput()
    {
        if (currentCrawlSettings == null || currentCrawlSettings.keyLists == null || currentCrawlSettings.keyLists.Length == 0)
        {
            Debug.LogWarning("无效的爬行设置或按键列表");
            return;
        }

        if (currentKeyIndex >= currentCrawlSettings.keyLists.Length)
        {
            currentKeyIndex = 0;
        }

        KeyList currentKeyList = currentCrawlSettings.keyLists[currentKeyIndex];
        
        if (IsValidKeyListInput(currentKeyList))
        {
            MovePlayer();
            currentKeyIndex++;

            if (currentKeyIndex >= currentCrawlSettings.keyLists.Length)
            {
                Debug.Log("重置 currentKeyIndex 为 0");
                currentKeyIndex = 0;
            }

            // 播放下一个动画关键帧
            PlayNextAnimationFrame(currentCrawlName);
        }
    }

    // 修改动画关键帧播放方法
    void PlayNextAnimationFrame(string crawlName)
    {
        if (currentCrawlSettings.animationFrames == null || currentCrawlSettings.animationFrames.Length == 0)
        {
            Debug.LogWarning($"{crawlName}没有设置动画帧！");
            return;
        }

        // 更新当前帧索引
        currentAnimFrame = (currentAnimFrame + 1) % currentCrawlSettings.TotalAnimFrames;
        
        // 获取当前帧
        AnimationFrame currentFrame = currentCrawlSettings.animationFrames[currentAnimFrame];
        
        // 更新精灵
        if (spriteRenderer != null && currentFrame.sprite != null)
        {
            spriteRenderer.sprite = currentFrame.sprite;
        }
        
        Debug.Log($"播放{crawlName}动画关键帧: {currentAnimFrame}/{currentCrawlSettings.TotalAnimFrames-1}");
    }

    // 添加碰撞检测方法
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 停止所有移动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        UpdateMoveDirection(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInContact)
        {
            // 确保在持续接触时也保持静止
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            UpdateMoveDirection(collision);
        }
        UpdateMoveDirection(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isInContact = false;
        canCrawlOnCurrentSurface = false;
    }

    // 添加调整旋转的方法
    void AdjustRotationToSurface(ContactPoint2D contact)
    {
        if (contact.collider == null) return;
        
        // 获取碰撞点的法线
        Vector2 normal = contact.normal;
        
        // 计算沿表面的移动方向（法线的垂直方向，即表面切线方向）
        Vector2 surfaceMoveDirection = new Vector2(-normal.y, normal.x);
        Debug.Log($"法线: {normal}, 原始表面方向: {surfaceMoveDirection}");
        
        // 确保移动方向与玩家的朝向一致
        if (Vector2.Dot(surfaceMoveDirection, Vector2.right) < 0)
        {
            surfaceMoveDirection = -surfaceMoveDirection;
            Debug.Log($"调整后的表面方向: {surfaceMoveDirection}");
        }

        // 根据移动方向计算旋转角度
        float angle = Mathf.Atan2(surfaceMoveDirection.y, surfaceMoveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // 根据isReversing调整scale，实现左右朝向
        Vector3 scale = transform.localScale;
        scale.x = isReversing ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        // 更新当前移动方向
        currentMoveDirection = surfaceMoveDirection;
        
        Debug.Log($"调整旋转 - 法线: {normal}, 移动方向: {surfaceMoveDirection}, 角度: {angle}");
    }

    void UpdateMoveDirection(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            isInContact = true;
            currentContact = collision.GetContact(0);
            
            // 调用新的旋转调整方法
            AdjustRotationToSurface(currentContact);
            CheckCrawlability();
        }
    }

    // 添加检查可爬行状态的方法
    private void CheckCrawlability()
    {
        if (currentContact.collider != null)
        {
            // 获取当前触表面的标签
            string surfaceTag = currentContact.collider.tag;
            
            // 通知 CrawlSurface 组件检查当前爬行类型是否可用
            CrawlSurface crawlSurface = currentContact.collider.GetComponent<CrawlSurface>();
            if (crawlSurface != null)
            {
                // 用当前的爬行类型重新检查可爬行状态
                crawlSurface.CheckCrawlabilityForType(currentCrawlName, this);
            }
        }
    }
}