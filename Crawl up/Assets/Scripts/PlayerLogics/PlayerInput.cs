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

        // 初始化基础爬行
        if (basicCrawl == null)
        {
            basicCrawl = new CrawlSettings();
        }
        basicCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.A}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.D}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 }
        };
        basicCrawl.isActive = true;
        basicCrawl.canCrawl = true;

        // 初始化壁虎爬行
        if (geckoCrawl == null)
        {
            geckoCrawl = new CrawlSettings();
        }
        geckoCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
        };
        geckoCrawl.isActive = false;
        geckoCrawl.canCrawl = false;

        // 初始化变色龙爬行
        if (chameleonCrawl == null)
        {
            chameleonCrawl = new CrawlSettings();
        }
        chameleonCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
        };
        chameleonCrawl.isActive = false;
        chameleonCrawl.canCrawl = false;

        // 初始化乌爬行
        if (turtleCrawl == null)
        {
            turtleCrawl = new CrawlSettings();
        }
        turtleCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.F, KeyCode.J }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.D, KeyCode.K }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
            new KeyList { keySequence = new KeyCode[] { KeyCode.S, KeyCode.L }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 }
        };
        turtleCrawl.isActive = false;
        turtleCrawl.canCrawl = false;

        // 初始化蛇爬行
        if (snakeCrawl == null)
        {
            snakeCrawl = new CrawlSettings();
        }
        snakeCrawl.keyLists = new KeyList[]
        {
            new KeyList { keySequence = new KeyCode[] { KeyCode.M } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.N } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.B } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.V } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.C } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.X } },
            new KeyList { keySequence = new KeyCode[] { KeyCode.Z } }
        };
        snakeCrawl.isActive = false;
        snakeCrawl.canCrawl = false;

        // 初始化猫爬行
        if (catCrawl == null)
        {
            catCrawl = new CrawlSettings();
        }
        catCrawl.keyLists = new KeyList[]
        {
            new KeyList { 
                keySequence = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G }, 
                requiredMinKeyCount = 3, 
                requiredMaxKeyCount = 5 
            },
            new KeyList { 
                keySequence = new KeyCode[] { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y }, 
                requiredMinKeyCount = 3, 
                requiredMaxKeyCount = 5 
            }
        };
        catCrawl.isActive = false;
        catCrawl.canCrawl = false;

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

            CrawlProgress progress = crawlProgresses[crawlType];
            
            if (progress.isInProgress && Time.time - progress.lastValidInputTime > maxTimeBetweenInputs)
            {
                ResetProgress(crawlType);
                continue;
            }

            // 根据 isReversing 获取正确的序列索引
            int sequenceIndex = isReversing 
                ? crawlSettings.keyLists.Length - 1 - progress.currentKeyListIndex 
                : progress.currentKeyListIndex;

            KeyList currentKeyList = crawlSettings.keyLists[sequenceIndex];

            if (IsValidKeyListInput(currentKeyList))
            {
                progress.lastValidInputTime = Time.time;
                progress.isInProgress = true;

                if (!progress.isPlayingTransitionAnim)
                {
                    progress.isPlayingTransitionAnim = true;
                    PlayTransitionAnimation(crawlType, sequenceIndex);
                }

                progress.currentKeyListIndex++;
                Debug.Log($"{crawlType} 进度: {progress.currentKeyListIndex}/{crawlSettings.keyLists.Length}");

                if (progress.currentKeyListIndex >= crawlSettings.keyLists.Length)
                {
                    Debug.Log($"切换到 {crawlType} 爬行方式");
                    ChangeCrawlType(crawlType);
                    ResetAllProgress();
                    return;
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
                // Debug.Log($"检测到无效按键: {pressedKey}，本次输入无效");
                return false;
            }
        }

        // 计算有效按键数量
        int keyCount = keyList.keySequence.Count(key => currentKeys.Contains(key));
        
        // Debug.Log($"按下的键数: {keyCount}, 最小要: {keyList.requiredMinKeyCount}, 最大允许: {keyList.requiredMaxKeyCount}");
        
        // 检查按键数是否在允许的范围内
        bool isValid = keyCount >= keyList.requiredMinKeyCount && keyCount <= keyList.requiredMaxKeyCount;
        if (isValid)
        {
            // Debug.Log($"符合要求的按键数: {keyCount}");
        }
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
        FieldInfo field = GetType().GetField(crawlType.ToLower() + "Crawl", BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            currentCrawlSettings = (CrawlSettings)field.GetValue(this);
            currentCrawlName = crawlType;

            // 新所有爬行方式的 isActive 状态
            basicCrawl.isActive = false;
            geckoCrawl.isActive = false;
            turtleCrawl.isActive = false;
            snakeCrawl.isActive = false;
            catCrawl.isActive = false;
            chameleonCrawl.isActive = false;

            currentCrawlSettings.isActive = true;

            // 如果当前正在接触表面，新检查爬行状态
            if (isInContact)
            {
                CheckCrawlability();
            }
        }
        ResetAllProgress();
    }

    private void OnEnable()
    {
        // 订阅事件
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_BASIC_CRAWL, OnLearnBasicCrawl);
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_GECKO_CRAWL, OnLearnGeckoCrawl);
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_TURTLE_CRAWL, OnLearnTurtleCrawl);
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_SNAKE_CRAWL, OnLearnSnakeCrawl);
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_CAT_CRAWL, OnLearnCatCrawl);
        EventCenter.Instance.Subscribe(EventCenter.EVENT_LEARNED_CHAMELEON_CRAWL, OnLearnChameleonCrawl);
    }

    private void OnDisable()
    {
        // 取消订阅事件
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_BASIC_CRAWL, OnLearnBasicCrawl);
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_GECKO_CRAWL, OnLearnGeckoCrawl);
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_TURTLE_CRAWL, OnLearnTurtleCrawl);
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_SNAKE_CRAWL, OnLearnSnakeCrawl);
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_CAT_CRAWL, OnLearnCatCrawl);
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_LEARNED_CHAMELEON_CRAWL, OnLearnChameleonCrawl);
    }

    private void OnDestroy()
    {
        // 确保在销毁时取消订阅事件
        OnDisable();
    }

    // 事件处理方法
    private void OnLearnBasicCrawl()
    {
        basicCrawl.canCrawl = true;
    }
    private void OnLearnGeckoCrawl()
    {
        geckoCrawl.canCrawl = true;
    }
    private void OnLearnTurtleCrawl()
    {
        turtleCrawl.canCrawl = true;
    }
    private void OnLearnSnakeCrawl()
    {
        snakeCrawl.canCrawl = true;
    }
    private void OnLearnCatCrawl()
    {
        catCrawl.canCrawl = true;
    }
    private void OnLearnChameleonCrawl()
    {
        chameleonCrawl.canCrawl = true;
    }

    // 添加新的辅助方法
    private void ResetProgress(string crawlType)
    {
        var progress = crawlProgresses[crawlType];
        progress.currentKeyListIndex = 0;
        progress.isInProgress = false;
        progress.lastValidInputTime = 0f;
        progress.isPlayingTransitionAnim = false;
        progress.currentTransitionFrame = 0;  // 重置过渡动画帧索引
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

        // 根据 isReversing 获取正确的序列索引
        int sequenceIndex = isReversing 
            ? currentCrawlSettings.keyLists.Length - 1 - currentKeyIndex 
            : currentKeyIndex;

        KeyList currentKeyList = currentCrawlSettings.keyLists[sequenceIndex];
        
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

    // 修改过渡动画播放方法
    void PlayTransitionAnimation(string crawlType, int sequenceIndex)
    {
        CrawlSettings settings = null;
        switch (crawlType.ToLower())
        {
            case "basic": settings = basicCrawl; break;
            case "gecko": settings = geckoCrawl; break;
            case "turtle": settings = turtleCrawl; break;
            case "snake": settings = snakeCrawl; break;
            case "cat": settings = catCrawl; break;
            case "chameleon": settings = chameleonCrawl; break;
        }

        if (settings == null || settings.transitionAnimation == null || 
            settings.transitionAnimation.transitionFrames == null || 
            settings.transitionAnimation.transitionFrames.Length != 2)
        {
            Debug.LogWarning($"{crawlType}没有设置过渡动画帧！");
            return;
        }

        var progress = crawlProgresses[crawlType];
        // 在两帧之间切换
        progress.currentTransitionFrame = (progress.currentTransitionFrame + 1) % 2;
        
        // 更新精灵
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = settings.transitionAnimation.transitionFrames[progress.currentTransitionFrame];
        }
        
        Debug.Log($"播放{crawlType}过渡动画帧: {progress.currentTransitionFrame}/1");
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