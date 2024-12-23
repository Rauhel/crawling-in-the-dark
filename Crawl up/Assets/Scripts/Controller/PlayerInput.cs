using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq; // 添加 LINQ 支持

[System.Serializable]
public class KeyList
{
    public KeyCode[] keySequence;
    public int requiredMinKeyCount = 1; // 最少需要按的键数
    public int requiredMaxKeyCount = 1; // 最多可以按的键数
}

[System.Serializable]
public class CrawlSettings
{
    public KeyList[] keyLists;
    public float groundSpeed;
    public float waterSpeed;
    public float iceSpeed;
    public float wallSpeed;
    public bool isActive; // 表示是否正在进行这种爬行
    public bool canCrawl; // 表示是否已经学会这种爬行
}

public class PlayerInput : MonoBehaviour
{
    public CrawlSettings basicCrawl;
    public CrawlSettings geckoCrawl;
    public CrawlSettings turtleCrawl;
    public CrawlSettings snakeCrawl;
    public CrawlSettings catCrawl;
    public CrawlSettings chameleonCrawl;

    public Animator animator;
    public float moveDistance = 1f;

    public int currentKeyIndex = 0;
    public CrawlSettings currentCrawlSettings;
    private string currentCrawlName;
    private bool isReversing = false;
    public List<KeyCode> currentKeys = new List<KeyCode>();
    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
    private bool keyDetected = false;

    void Start()
    {
        currentKeyIndex = 0;
        ChangeCrawlType("Basic");

        // Set default crawl settings
        basicCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.A}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.D}, requiredMinKeyCount = 1, requiredMaxKeyCount = 1 }
            },
            groundSpeed = 5f, // 根据需要设置速度
            waterSpeed = 2f,
            iceSpeed = 1.5f,
            wallSpeed = 3f,
            isActive = false,
            canCrawl = false
        };

        // 定义 GeckoCrawl 的按键顺序
        geckoCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.W } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.R } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.S } },
                new KeyList { keySequence = new KeyCode[] { KeyCode.F } }
            },
            groundSpeed = 5f, // 根据需要设置速度
            waterSpeed = 3f,
            iceSpeed = 2f,
            wallSpeed = 4f,
            isActive = false,
            canCrawl = false
        };

        // 定义 TurtleCrawl 的按键顺序
        turtleCrawl = new CrawlSettings
        {
            keyLists = new KeyList[]
            {
                new KeyList { keySequence = new KeyCode[] { KeyCode.F, KeyCode.J }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.D, KeyCode.K }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 },
                new KeyList { keySequence = new KeyCode[] { KeyCode.S, KeyCode.L }, requiredMinKeyCount = 2, requiredMaxKeyCount = 2 }
            },
            groundSpeed = 2f, // 根据需要设置速度
            waterSpeed = 1f,
            iceSpeed = 1.5f,
            wallSpeed = 1f,
            isActive = false,
            canCrawl = false
        };

        // 定义 SnakeCrawl 的按键顺序
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
            groundSpeed = 3f, // 根据需要设置速度
            waterSpeed = 1.5f,
            iceSpeed = 1f,
            wallSpeed = 2f,
            isActive = false,
            canCrawl = false
        };

        // 定义 CatCrawl 的按键序列
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
            groundSpeed = 4f, // 根据需要设置速度
            waterSpeed = 2f,
            iceSpeed = 1.5f,
            wallSpeed = 3f,
            isActive = false,
            canCrawl = false
        };
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
        // 检查空格键是否被按下
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 按一下空格键，isReversing的值在true和false之间切换
            isReversing = !isReversing;
        }
    }

    void CheckInput()
    {
        // 检查是否是每种爬行方式的第一个或最后一个按键序列
        foreach (var crawlType in new[] { "Basic", "Gecko", "Turtle", "Snake", "Cat", "Chameleon" })
        {
            CrawlSettings crawlSettings = (CrawlSettings)GetType().GetField(crawlType.ToLower() + "Crawl", BindingFlags.Public | BindingFlags.Instance).GetValue(this);
            if (crawlSettings.canCrawl && crawlSettings.keyLists != null && crawlSettings.keyLists.Length > 0) // 确保数组不为空
            {
                KeyList firstKeyList = crawlSettings.keyLists[0];
                KeyList lastKeyList = crawlSettings.keyLists[crawlSettings.keyLists.Length - 1];

                // 检查是否按下了第一个或最后一个按键序列的键
                if (Input.GetKeyDown(firstKeyList.keySequence[0]) || Input.GetKeyDown(lastKeyList.keySequence[0]))
                {
                    Debug.Log("Crawl type switched: " + crawlType); // 打印切换的爬行类型
                    ChangeCrawlType(crawlType);
                    Debug.Log("Key pressed for crawl type: " + crawlType); // 打印按下的爬行类型
                    return; // 退出方法，因为已经处理了爬行方式的切换
                }
                

            }
            else if (!crawlSettings.canCrawl)
            {
                // pass
            }
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
        
        // 检查是否存在无效按键（不在当前序列中的按键）
        foreach (KeyCode pressedKey in currentKeys)
        {
            if (!currentKeyList.keySequence.Contains(pressedKey))
            {
                Debug.Log($"检测到无效按键: {pressedKey}，本次输入无效");
                return; // 如果有任何一个按键不在序列中，直接返回
            }
        }

        // 计算有效按键数量
        int keyCount = 0;
        foreach (KeyCode key in currentKeyList.keySequence)
        {
            if (currentKeys.Contains(key))
            {
                keyCount++;
            }
        }

        Debug.Log($"按下的键数: {keyCount}, 最小要求: {currentKeyList.requiredMinKeyCount}, 最大允许: {currentKeyList.requiredMaxKeyCount}");
        
        // 检查按键数是否在允许的范围内
        if (keyCount >= currentKeyList.requiredMinKeyCount && keyCount <= currentKeyList.requiredMaxKeyCount)
        {
            Debug.Log($"符合要求的按键数: {keyCount}");
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

    void MovePlayer()
    {
        Debug.Log("Moving player...");
        float speed = GetSpeedBasedOnSurface();
        Vector3 moveDirection = isReversing ? Vector3.left : Vector3.right;
        transform.position += moveDirection * speed * moveDistance * Time.deltaTime;
    }

    float GetSpeedBasedOnSurface()
    {
        return currentCrawlSettings.groundSpeed;
    }

    void PlayAnimation(string crawlName)
    {
        animator.SetTrigger(crawlName);
    }

    // Call this method to change the current crawl settings
    public void ChangeCrawlType(string crawlType)
    {
        FieldInfo field = GetType().GetField(crawlType.ToLower() + "Crawl", BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            currentCrawlSettings = (CrawlSettings)field.GetValue(this);
            currentCrawlName = crawlType;

            // 更新所有爬行方式的 isActive 状态
            basicCrawl.isActive = false;
            geckoCrawl.isActive = false;
            turtleCrawl.isActive = false;
            snakeCrawl.isActive = false;
            catCrawl.isActive = false;
            chameleonCrawl.isActive = false;

            currentCrawlSettings.isActive = true;
        }
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
}