using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    public CrawlSettings basicCrawl;
    public CrawlSettings geckoCrawl;
    public CrawlSettings turtleCrawl;
    public CrawlSettings snakeCrawl;
    public CrawlSettings catCrawl;
    public CrawlSettings chameleonCrawl;

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

    // 在现有变量后添加
    private Dictionary<string, CrawlProgress> crawlProgresses = new Dictionary<string, CrawlProgress>();
    public float maxTimeBetweenInputs = 2f;  // 输入序列的最大间隔时间
    
    // 添加新的字段
    private Vector2 currentMoveDirection = Vector2.right;  // 默认向右移动
    private ContactPoint2D currentContact;  // 存储当前的接触点信息
    private bool isInContact = false;  // 是否正在接触表面
    public bool canCrawlOnCurrentSurface = false;  // 是否可以在当前表面爬行

    void Start()
    {
        currentKeyIndex = 0;
        ChangeCrawlType("Basic");

        // Basic Crawl
        basicCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.A}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.D}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 }
            },
            groundSpeed = 50f,    // 10f * 5
            waterSpeed = 0f,
            iceSpeed = 10f,      // 2f * 5
            wallSpeed = 0f,
            isActive = true,
            canCrawl = true
        };

        // Gecko Crawl
        geckoCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
            },
            groundSpeed = 50f,    // 10f * 5
            waterSpeed = 30f,     // 6f * 5
            iceSpeed = 20f,      // 4f * 5
            wallSpeed = 40f,     // 8f * 5
            isActive = false,
            canCrawl = false
        };

        // Chameleon Crawl
        chameleonCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
            },
            groundSpeed = 30f,    // 6f * 5
            waterSpeed = 0f,
            iceSpeed = 10f,      // 2f * 5
            wallSpeed = 0f,
            isActive = false,
            canCrawl = false
        };

        // Turtle Crawl
        turtleCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.F, KeyCode.J }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.D, KeyCode.K }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.S, KeyCode.L }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 }
            },
            groundSpeed = 20f,    // 4f * 5
            waterSpeed = 60f,     // 12f * 5
            iceSpeed = 40f,      // 8f * 5
            wallSpeed = 0f,
            isActive = false,
            canCrawl = false
        };

        // Snake Crawl
        snakeCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.M } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.N } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.B } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.V } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.C } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.X } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.Z } }
            },
            groundSpeed = 30f,    // 6f * 5
            waterSpeed = 30f,     // 6f * 5
            iceSpeed = 20f,      // 4f * 5
            wallSpeed = 50f,     // 10f * 5
            isActive = false,
            canCrawl = false
        };

        // Cat Crawl
        catCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
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
            },
            groundSpeed = 50f,    // 10f * 5
            waterSpeed = 0f,
            iceSpeed = 0f,
            wallSpeed = 50f,     // 10f * 5
            isActive = false,
            canCrawl = false
        };

        // 初始化进度字典
        foreach (var crawlType in new[] { "Basic", "Gecko", "Turtle", "Snake", "Cat", "Chameleon" })
        {
            crawlProgresses[crawlType] = new CrawlProgress();
        }
    }

    void Update()
    {
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
    }

    void CheckSpaceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isReversing = !isReversing;
            // 当切换方向时重置所有进度
            ResetAllProgress();
            currentKeyIndex = 0;
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
        
        // Debug.Log($"按下的键数: {keyCount}, 最小要求: {keyList.requiredMinKeyCount}, 最大允许: {keyList.requiredMaxKeyCount}");
        
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
        Vector3 moveDirection = isReversing ? -currentMoveDirection : currentMoveDirection;
        transform.position += (Vector3)moveDirection * speed * moveDistance * Time.deltaTime;
    }

    float GetSpeedBasedOnSurface()
    {
        return currentCrawlSettings.groundSpeed;
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

            // �����新所有爬行方式的 isActive 状态
            basicCrawl.isActive = false;
            geckoCrawl.isActive = false;
            turtleCrawl.isActive = false;
            snakeCrawl.isActive = false;
            catCrawl.isActive = false;
            chameleonCrawl.isActive = false;

            currentCrawlSettings.isActive = true;

            // 如果当前正在接触表面，重新检查可爬行状态
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
        progress.isPlayingTransitionAnim = false;  // 重置动画状态
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
            PlayAnimation(currentCrawlName);
        }
    }

    // 添加新的动画播放方法
    void PlayTransitionAnimation(string crawlType, int sequenceIndex)
    {
        // 注释掉过渡动画相关代码
        // string triggerName = $"{crawlType}Transition{sequenceIndex + 1}";
        // animator.SetTrigger(triggerName);
        Debug.Log($"播放过渡动画: {crawlType}Transition{sequenceIndex + 1}");
    }

    // 添加碰撞检测方法
    void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateMoveDirection(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!isInContact)
        {
            UpdateMoveDirection(collision);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isInContact = false;
        canCrawlOnCurrentSurface = false;  // 离开表面时重置爬行状态
    }

    // 添加更新移动方向的方法
    void UpdateMoveDirection(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            isInContact = true;
            currentContact = collision.GetContact(0);
            
            // 获取碰撞点的法线
            Vector2 normal = currentContact.normal;
            
            // ��算��线方向（顺时针）
            currentMoveDirection = new Vector2(normal.y, -normal.x);
            
            // 确保切线方向总是指向右侧（如果可能）
            if (currentMoveDirection.x < 0)
            {
                currentMoveDirection = -currentMoveDirection;
            }

            // 在更新移动方向时也检查可爬行状态
            CheckCrawlability();
            
            Debug.Log($"新的移动方向: {currentMoveDirection}");
        }
    }

    // 添加检查可爬行状态的方法
    private void CheckCrawlability()
    {
        if (currentContact.collider != null)
        {
            // 获取当前接触表面的标签
            string surfaceTag = currentContact.collider.tag;
            
            // 通知 CrawlSurface 组件检查当前爬行类型是否可用
            CrawlSurface crawlSurface = currentContact.collider.GetComponent<CrawlSurface>();
            if (crawlSurface != null)
            {
                // 使用当前的爬行类型重新检查可爬行状态
                crawlSurface.CheckCrawlabilityForType(currentCrawlName, this);
            }
        }
    }
}