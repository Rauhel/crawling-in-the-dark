using UnityEngine;
using System.Reflection;

[System.Serializable]
public class KeySequence
{
    public KeyCode key;
    public KeyCode[] parallelKeys;
    public int requiredKeyCount = 1; // 默认需要按1个键
}

[System.Serializable]
public class CrawlSettings
{
    public KeySequence[] keySequence;
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

    private int currentKeyIndex = 0;
    private CrawlSettings currentCrawlSettings;
    private string currentCrawlName;
    private bool isReversing = false;
    public KeyCode currentKey;

    void Start()
    {
        // Set default crawl settings
        ChangeCrawlType("Basic");

        // 定义 BasicCrawl 的按键顺序
        basicCrawl = new CrawlSettings
        {
            keySequence = new KeySequence[]
            {
                new KeySequence { key = KeyCode.A },
                new KeySequence { key = KeyCode.D }
            },
            groundSpeed = 4f, // 根据需要设置速度
            waterSpeed = 2f,
            iceSpeed = 1.5f,
            wallSpeed = 3f,
            isActive = false,
            canCrawl = false
        };

        // 定义 GeckoCrawl 的按键顺序
        geckoCrawl = new CrawlSettings
        {
            keySequence = new KeySequence[]
            {
                new KeySequence { key = KeyCode.W },
                new KeySequence { key = KeyCode.R },
                new KeySequence { key = KeyCode.S },
                new KeySequence { key = KeyCode.F }
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
            keySequence = new KeySequence[]
            {
                new KeySequence
                {
                    parallelKeys = new KeyCode[]
                    {
                        KeyCode.F, KeyCode.J
                    },
                    requiredKeyCount = 2
                },
                new KeySequence
                {
                    parallelKeys = new KeyCode[]
                    {
                        KeyCode.D, KeyCode.K
                    },
                    requiredKeyCount = 2
                },
                new KeySequence
                {
                    parallelKeys = new KeyCode[]
                    {
                        KeyCode.S, KeyCode.L
                    },
                    requiredKeyCount = 2
                }
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
            keySequence = new KeySequence[]
            {
                new KeySequence { key = KeyCode.M },
                new KeySequence { key = KeyCode.N },
                new KeySequence { key = KeyCode.B },
                new KeySequence { key = KeyCode.V },
                new KeySequence { key = KeyCode.C },
                new KeySequence { key = KeyCode.X },
                new KeySequence { key = KeyCode.Z }
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
            keySequence = new KeySequence[]
            {
                new KeySequence
                {
                    parallelKeys = new KeyCode[]
                    {
                        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
                        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
                        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B
                    },
                    requiredKeyCount = 5
                },
                new KeySequence
                {
                    parallelKeys = new KeyCode[]
                    {
                        KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y,
                        KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N,
                        KeyCode.M
                    },
                    requiredKeyCount = 5
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
        CheckInput();
        CheckSpaceInput(); // 检查空格键输入
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                currentKey = key;
                break;
            }
        }
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
            if (crawlSettings.canCrawl && crawlSettings.keySequence != null && crawlSettings.keySequence.Length > 0) // 确保数组不为空
            {
                KeySequence firstKeySequence = crawlSettings.keySequence[0];
                KeySequence lastKeySequence = crawlSettings.keySequence[crawlSettings.keySequence.Length - 1];

                // 检查是否按下了第一个或最后一个按键序列的键
                if (Input.GetKeyDown(firstKeySequence.key) || Input.GetKeyDown(lastKeySequence.key))
                {
                    Debug.Log("Crawl type switched: " + crawlType); // 打印切换的爬行类型
                    ChangeCrawlType(crawlType);
                    currentKeyIndex = 0; // 重置索引
                    return; // 退出方法，因为已经处理了爬行方式的切换
                    Debug.Log("Key pressed for crawl type: " + crawlType); // 打印按下的爬行类型
                }
            }
            else if (!crawlSettings.canCrawl)
            {
                // pass
            }
        }
        // 如果当前爬行设置的isActive为true，则处理输入
        Debug.Log($"currentCrawlSettings: {(currentCrawlSettings == null ? "null" : "not null")}");
        if (currentCrawlSettings != null)
        {
            // Debug.Log($"isActive: {currentCrawlSettings.isActive}");
            // Debug.Log($"currentKeyIndex: {currentKeyIndex}");
            // Debug.Log($"keySequence length: {currentCrawlSettings.keySequence.Length}");
        }

        if (currentCrawlSettings != null && currentCrawlSettings.isActive && currentKeyIndex >= 0 && currentKeyIndex < currentCrawlSettings.keySequence.Length)
        {
            // Debug.Log("Current crawl settings are valid, processing input...");
            KeySequence currentKeySequence = currentCrawlSettings.keySequence[currentKeyIndex];
            // Debug.Log($"Current key sequence at index {currentKeyIndex}");
            // Debug.Log($"Current key sequence: {currentKeySequence.key}");

            
            if (currentKeySequence.parallelKeys != null && currentKeySequence.parallelKeys.Length > 0)
            {
                // Debug.Log($"Processing parallel input with {currentKeySequence.parallelKeys.Length} keys");
                CheckParallelInput(currentKeySequence);
            }
            else
            {
                // Debug.Log("Processing sequential input");
                Debug.Log("CHHHECKKKKKING SEQUENTIAL INPUT");
                CheckSequentialInput(currentKeySequence);
            }
        }
        else
        {
            Debug.Log("Skipping input processing - invalid crawl settings or key index");
        }
    }

    void CheckSequentialInput(KeySequence keySequence)
    {
        // Debug.Log("Entering CheckSequentialInput");
        // Debug.Log($"isReversing: {isReversing}");
        // Debug.Log($"currentKeyIndex: {currentKeyIndex}");
        
        // 如果isReversing为true，则检查是否按下了最后一个按键序列的键，否则检查第一个
        if (isReversing)
        {
            Debug.Log("Checking reversed input");
            KeyCode expectedKey = currentCrawlSettings.keySequence[currentCrawlSettings.keySequence.Length - 1 - currentKeyIndex].key;
            Debug.Log($"Expected key in reverse: {expectedKey}");
            
            if (Input.GetKeyDown(expectedKey))
            {
                Debug.Log($"Correct key pressed in reverse: {expectedKey}");
                MovePlayer();
                currentKeyIndex++;
                Debug.Log($"Increased currentKeyIndex to: {currentKeyIndex}");
                
                if (currentKeyIndex >= currentCrawlSettings.keySequence.Length)
                {
                    Debug.Log("Resetting currentKeyIndex to 0");
                    currentKeyIndex = 0;
                }
                PlayAnimation(currentCrawlName);
            }
        }
        else
        {
            Debug.Log("Checking forward input");
            Debug.Log($"Expected key: {keySequence.key}");
            
            // if (Input.GetKeyDown(keySequence.key))
            if (currentKey == keySequence.key)
            {
                Debug.LogWarning($"Correct key pressed: {keySequence.key}");
                MovePlayer();
                currentKeyIndex++;
                Debug.Log($"Increased currentKeyIndex to: {currentKeyIndex}");
                
                if (currentKeyIndex >= currentCrawlSettings.keySequence.Length)
                {
                    Debug.Log("Resetting currentKeyIndex to 0");
                    currentKeyIndex = 0;
                }
                PlayAnimation(currentCrawlName);
            }
        }
        Debug.Log("Exiting CheckSequentialInput");
    }

    void CheckParallelInput(KeySequence keySequence)
    {
        if (isReversing)
        {
            // 如果isReversing为true，则逆序检查并行按键
            int keyCount = 0;
            for (int i = keySequence.parallelKeys.Length - 1; i >= 0; i--) // 逆序遍历数组
            {
                if (Input.GetKey(keySequence.parallelKeys[i]))
                {
                    keyCount++;
                }
            }

            if (keyCount >= keySequence.requiredKeyCount)
            {
                MovePlayer();
                currentKeyIndex++;
                if (currentKeyIndex >= currentCrawlSettings.keySequence.Length)
                {
                    currentKeyIndex = 0;
                }
                PlayAnimation(currentCrawlName);
            }
        }
        else
        {
            // 如果isReversing为false，则正序检查并行按键
            int keyCount = 0;
            foreach (KeyCode key in keySequence.parallelKeys)
            {
                if (Input.GetKey(key))
                {
                    keyCount++;
                }
            }

            if (keyCount >= keySequence.requiredKeyCount)
            {
                MovePlayer();
                currentKeyIndex++;
                if (currentKeyIndex >= currentCrawlSettings.keySequence.Length)
                {
                    currentKeyIndex = 0;
                }
                PlayAnimation(currentCrawlName);
            }
        }
    }

    void MovePlayer()
    {
        Debug.Log("Moving player");
        float speed = GetSpeedBasedOnSurface();
        Vector3 moveDirection = isReversing ? Vector3.left : Vector3.right;
        transform.position += moveDirection * speed * moveDistance * Time.deltaTime;
    }

    float GetSpeedBasedOnSurface()
    {
        // This function should return the appropriate speed based on the surface the player is on.
        // For simplicity, it returns the ground speed. You should implement the logic to determine the surface.
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