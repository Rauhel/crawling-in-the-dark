using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NpcPatrol : MonoBehaviour
{
    [Header("基础设置")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 10f;
    public float escapeSpeed = 12f;  // 逃跑速度
    public float knockbackForce = 10f;  // 被击中时的弹飞力度
    [Tooltip("水平检测范围")]
    public float horizontalDetectionRange = 8f;
    [Tooltip("垂直检测范围")]
    public float verticalDetectionRange = 3f;
    [Tooltip("击杀检测范围")]
    public float bodyRange = 1f;      // 新增：触发击杀的范围
    public float knockbackDistance = 8f;  // 弹飞距离，增加到8
    public float knockbackDuration = 0.2f;  // 弹飞持续时间，减少到0.2秒

    [Header("追逐设置")]
    [Tooltip("追逐相关的设置")]
    public DetectionSettings detectionSettings;

    [Header("巡逻路径设置")]
    [Tooltip("引用场景中的PatrolPath物体")]
    public PatrolPath patrolPath;
    private float patrolProgress = 0f;  // 0到1之间的值
    private bool isPathReversing = false;

    [Header("NPC设置")]
    public string npcType;  // 在Inspector中设置为"Frog"/"Chameleon"/"Cat"/"Turtle"/"Snake"
    public bool isSafePointNPC = false;  // 是否是安全点NPC
    public bool isTeacherNPC = false;    // 是否是教学NPC
    [Tooltip("如果是教学NPC，选择要教授的爬行类型")]
    public CrawlType teachableCrawlType;  // 要教授的爬行类型

    [Header("对话触发设置")]
    public DialogueTrigger[] dialogueTriggers;  // 在Inspector中设置对话触发条件

    // 私有状态字段
    private bool isDead = false;
    private bool isChasing = false;
    private bool isInDialogue = false;
    private bool canStartDialogue = false;
    private int currentDialogueIndex = -1;
    private Transform playerTransform;
    private Vector3 originalPosition;
    private bool isEscaping = false;  // 是否在逃跑
    private bool isKnockedBack = false;  // 是否被弹飞
    private Vector3 deadPosition = new Vector3(1000, 1000, 0);  // 死亡时移动到的位置
    private Vector3 knockbackStartPos;
    private Vector3 knockbackTargetPos;
    private float knockbackTimer = 0f;

    void Start()
    {
        originalPosition = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead && !isKnockedBack) return;

        // 处理弹飞效果
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            float t = knockbackTimer / knockbackDuration;
            Debug.Log($"弹飞进行中: timer={knockbackTimer}, t={t}, 当前位置={transform.position}");
            
            if (t >= 1f)
            {
                Debug.Log("弹飞结束，移动到死亡位置");
                isKnockedBack = false;
                transform.position = deadPosition;
            }
            else
            {
                // 使用平滑的插值
                transform.position = Vector3.Lerp(knockbackStartPos, knockbackTargetPos, t);
            }
            return;
        }

        if (playerTransform != null)
        {
            // 计算玩家相对于NPC的位置差
            Vector2 toPlayer = playerTransform.position - transform.position;
            // 计算椭圆检测
            float normalizedDistance = Mathf.Pow(toPlayer.x / horizontalDetectionRange, 2) + 
                                    Mathf.Pow(toPlayer.y / verticalDetectionRange, 2);
            
            // 计算实际距离（用于bodyRange检测）
            float actualDistance = Vector2.Distance(transform.position, playerTransform.position);
            
            PlayerInput playerInput = playerTransform.GetComponent<PlayerInput>();
            
            if (normalizedDistance <= 1f)  // 如果在椭圆范围内
            {
                bool shouldChase = CheckPlayerDetection(playerInput);
                bool shouldEscape = CheckPlayerLethal(playerInput);
                
                if (shouldEscape)
                {
                    if (actualDistance <= bodyRange)
                    {
                        Debug.Log($"玩家进入击杀范围，距离={actualDistance}，bodyRange={bodyRange}");
                        // 被玩家击中，触发弹飞和死亡
                        StartKnockback(toPlayer.normalized);
                        Die();
                        return;
                    }
                    // 注释掉逃跑相关代码
                    // EscapeFromPlayer();
                }
                else if (shouldChase)
                {
                    if (actualDistance <= bodyRange)
                    {
                        KillPlayer();
                        return;
                    }
                    ChasePlayer();
                }
                else if (canStartDialogue && Input.GetKeyDown(KeyCode.Q))
                {
                    StartDialogue();
                    return;
                }
            }
            else
            {
                ResetDetectionState();
            }
        }

        // 只有在不能对话且不在追击状态时才巡逻
        if (!canStartDialogue && !isChasing && !isInDialogue)
        {
            Patrol();
        }
    }

    private void ResetDetectionState()
    {
        if (canStartDialogue)
        {
            // 如果之前可以对话，现在重置状态时隐藏提示
            PromptManager.Instance.HideInteractionPrompt();
        }
        isChasing = false;
        isEscaping = false;
        canStartDialogue = false;
        currentDialogueIndex = -1;
    }

    void ChasePlayer()
    {
        if (patrolPath == null) return;

        Debug.Log("ChasePlayer");
        isChasing = true;

        // 获取路径的两个端点
        Vector3 forwardEnd, backwardEnd;
        patrolPath.GetEndPoints(out forwardEnd, out backwardEnd);
        
        // 计算玩家到两个端点的距离
        float distToForward = Vector2.Distance(playerTransform.position, forwardEnd);
        float distToBackward = Vector2.Distance(playerTransform.position, backwardEnd);
        
        // 获取路径总长度用于计算速度
        float totalLength = patrolPath.GetTotalPathLength();
        float progressStep = (chaseSpeed * Time.deltaTime) / totalLength;
        
        // 根据玩家位置决定移动方向
        bool shouldMoveForward = distToForward < distToBackward;
        
        // 更新进度
        if (shouldMoveForward)
        {
            patrolProgress = Mathf.Min(1f, patrolProgress + progressStep);
        }
        else
        {
            patrolProgress = Mathf.Max(0f, patrolProgress - progressStep);
        }
        
        // 获取目标位置并移动
        Vector3 targetPosition = patrolPath.GetPositionAtProgress(patrolProgress);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        
        // 更新朝向
        Vector3 scale = transform.localScale;
        scale.x = shouldMoveForward ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Patrol()
    {
        if (patrolPath == null) return;

        // 获取路径总长度用于计算速度
        float totalLength = patrolPath.GetTotalPathLength();
        float progressStep = (patrolSpeed * Time.deltaTime) / totalLength;
        
        bool wasReversing = isPathReversing;
        
        // 更新进度
        if (isPathReversing)
        {
            patrolProgress = Mathf.Max(0f, patrolProgress - progressStep);
            if (patrolProgress <= 0f)
            {
                isPathReversing = false;
                patrolProgress = 0f;
            }
        }
        else
        {
            patrolProgress = Mathf.Min(1f, patrolProgress + progressStep);
            if (patrolProgress >= 1f)
            {
                isPathReversing = true;
                patrolProgress = 1f;
            }
        }

        // 如果方向发生改变，立即调朝向
        if (wasReversing != isPathReversing)
        {
            Vector3 scale = transform.localScale;
            scale.x = isPathReversing ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        // 获取目标位置并移动
        Vector3 targetPosition = patrolPath.GetPositionAtProgress(patrolProgress);
        
        // 平滑移动
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;  // 如果NPC已死亡，不处理碰撞

        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                CheckPlayerDetection(playerInput);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead || isInDialogue) return;  // 如果NPC已死亡或正在对话，不处理碰撞

        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                CheckPlayerDetection(playerInput);
            }
        }
    }

    private void KillPlayer()
    {
        Debug.Log($"执行KillPlayer - NPC位置: {transform.position}");
        // 通知 EventCenter 玩家死亡
        EventCenter.Instance.Publish(EventCenter.EVENT_PLAYER_DIED);
        Debug.Log("已发送玩家死亡事件");
        
        // 重置追击状态
        isChasing = false;
    }

    private void Die()
    {
        // 设置状态但不立即移动位置，让弹飞效果完成
        isDead = true;
        isChasing = false;
        isEscaping = false;
        Debug.Log("NPC 死亡了！");
        
        // 通知事件中心NPC死亡
        EventCenter.Instance.Publish(EventCenter.EVENT_NPC_DIED);
    }

    // 添加复活方法
    public void Revive()
    {
        isDead = false;
        isChasing = false;
        isEscaping = false;
        isKnockedBack = false;
        knockbackTimer = 0f;
        transform.position = originalPosition;
        Debug.Log("NPC 已复活！");
    }

    private float GetPlayerCurrentSpeed(PlayerInput playerInput)
    {
        // 直接返回玩家当前的实际速度
        return playerInput.CurrentActualSpeed;
    }

    bool CheckPlayerDetection(PlayerInput playerInput)
    {
        if (isDead || playerInput == null || isInDialogue) return false;

        if (System.Enum.TryParse<CrawlType>(playerInput.currentCrawlName, true, out CrawlType currentCrawlType))
        {
            // 检查是否有可触发的对话
            DialogueTrigger trigger = System.Array.Find(dialogueTriggers, 
                t => t.requiredCrawlType == currentCrawlType && !t.hasTriggered);
            
            if (trigger != null)
            {
                Debug.Log($"找到匹配的对话触发: {currentCrawlType}, 对话索引: {trigger.dialogueIndex}");
                canStartDialogue = true;
                currentDialogueIndex = trigger.dialogueIndex;
                isChasing = false;

                // 通知PromptManager显示交互提示
                PromptManager.Instance.ShowInteractionPrompt();
                
                return false;
            }

            // 检查是否是敌对爬行类型
            bool isHostileCrawl = System.Array.Exists(detectionSettings.hostileCrawlTypes, 
                type => type == currentCrawlType);

            // 如果条件改变，重置追击状态
            if (!isHostileCrawl)
            {
                if (isChasing)
                {
                    Debug.Log("玩家状态改变，停止追击");
                    ResetDetectionState();
                }
                return false;
            }

            // 满足追击条件
            if (!isChasing)
            {
                Debug.Log("开始追击玩家");
            }
            isChasing = true;
            canStartDialogue = false;
            currentDialogueIndex = -1;
            return true;
        }

        return false;
    }

    private void StartDialogue()
    {
        if (currentDialogueIndex < 0) return;

        if (DialogueData.Instance == null)
        {
            Debug.LogError("DialogueData.Instance 为空！请确保场景中有 DialogueSystem 预制体");
            return;
        }

        // 从DialogueData获取对话内容
        string[] lines = DialogueData.Instance.GetDialogue(npcType, currentDialogueIndex);
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError($"找不到NPC {npcType} 的对话内容！索引: {currentDialogueIndex}");
            return;
        }

        isInDialogue = true;
        canStartDialogue = false;
        isChasing = false;

        // 通知PromptManager隐藏交互提示并开始对话
        PromptManager.Instance.HideInteractionPrompt();
        DialogueManager.Instance.StartDialogue(
            lines,
            () => {
                // 对话结束后的回调
                isInDialogue = false;
                
                // 标记当前对话已触发
                var trigger = System.Array.Find(dialogueTriggers, 
                    t => t.dialogueIndex == currentDialogueIndex);
                if (trigger != null)
                {
                    trigger.hasTriggered = true;
                }

                // 如果是安全点NPC，设置安全点
                if (isSafePointNPC)
                {
                    PlayerManager.Instance.SetSafePoint(transform.position);
                }

                // 如果是教学NPC，触发学习事件
                if (isTeacherNPC)
                {
                    TeachCrawlType();
                }

                currentDialogueIndex = -1;  // 重置对话索引
            }
        );
    }

    private void TeachCrawlType()
    {
        string eventType = "";
        switch (teachableCrawlType)
        {
            case CrawlType.Basic:
                eventType = EventCenter.EVENT_LEARNED_BASIC_CRAWL;
                break;
            case CrawlType.Gecko:
                eventType = EventCenter.EVENT_LEARNED_GECKO_CRAWL;
                break;
            case CrawlType.Turtle:
                eventType = EventCenter.EVENT_LEARNED_TURTLE_CRAWL;
                break;
            case CrawlType.Snake:
                eventType = EventCenter.EVENT_LEARNED_SNAKE_CRAWL;
                break;
            case CrawlType.Cat:
                eventType = EventCenter.EVENT_LEARNED_CAT_CRAWL;
                break;
            case CrawlType.Chameleon:
                eventType = EventCenter.EVENT_LEARNED_CHAMELEON_CRAWL;
                break;
        }

        if (!string.IsNullOrEmpty(eventType))
        {
            EventCenter.Instance.Publish(eventType);
            Debug.Log($"玩家学会了 {teachableCrawlType} 爬行方式！");
        }
    }

    // 在 Scene 视图中绘制检测范围和巡逻区域
    void OnDrawGizmos()
    {
        // 绘制椭圆形检测范围
        const int segments = 50;
        Vector3 prevPos = Vector3.zero;
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * horizontalDetectionRange;
            float y = Mathf.Sin(angle) * verticalDetectionRange;
            Vector3 pos = transform.position + new Vector3(x, y, 0);
            
            if (i > 0)
            {
                Gizmos.DrawLine(prevPos, pos);
            }
            prevPos = pos;
        }
        
        // 绘制击杀范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bodyRange);
    }

    private void OnEnable()
    {
        // 注册到NpcManager
        if (NpcManager.Instance != null)
            NpcManager.Instance.RegisterNpc(this);
    }

    private void OnDisable()
    {
        // 从NpcManager中注销
        if (NpcManager.Instance != null)
            NpcManager.Instance.UnregisterNpc(this);
    }

    bool CheckPlayerLethal(PlayerInput playerInput)
    {
        if (playerInput != null && playerInput.currentCrawlSettings != null)
        {
            CrawlType currentCrawlType;
            if (System.Enum.TryParse<CrawlType>(playerInput.currentCrawlName, true, out currentCrawlType))
            {
                return System.Array.Exists(detectionSettings.lethalCrawlTypes, type => type == currentCrawlType);
            }
        }
        return false;
    }

    void EscapeFromPlayer()
    {
        if (patrolPath == null) return;

        Debug.Log("Escaping from player");
        isEscaping = true;
        isChasing = false;

        // 获取路径的两个端点
        Vector3 forwardEnd, backwardEnd;
        patrolPath.GetEndPoints(out forwardEnd, out backwardEnd);
        
        // 计算玩家到两个端点的距离
        float distToForward = Vector2.Distance(playerTransform.position, forwardEnd);
        float distToBackward = Vector2.Distance(playerTransform.position, backwardEnd);
        
        // 获取路径总长度用于计算速度
        float totalLength = patrolPath.GetTotalPathLength();
        float progressStep = (escapeSpeed * Time.deltaTime) / totalLength;
        
        // 选择离玩家更远的方向逃跑
        bool shouldMoveForward = distToForward > distToBackward;
        
        // 更新进度
        if (shouldMoveForward)
        {
            patrolProgress = Mathf.Min(1f, patrolProgress + progressStep);
        }
        else
        {
            patrolProgress = Mathf.Max(0f, patrolProgress - progressStep);
        }
        
        // 获取目标位置并移动
        Vector3 targetPosition = patrolPath.GetPositionAtProgress(patrolProgress);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, escapeSpeed * Time.deltaTime);
        
        // 更新朝向
        Vector3 scale = transform.localScale;
        scale.x = shouldMoveForward ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void StartKnockback(Vector2 direction)
    {
        Debug.Log($"开始弹飞，方向={direction}, 距离={knockbackDistance}");
        Debug.Log($"起始位置={transform.position}");
        isKnockedBack = true;
        knockbackTimer = 0f;
        knockbackStartPos = transform.position;
        knockbackTargetPos = (Vector2)transform.position - direction * knockbackDistance;
        Debug.Log($"目标位置={knockbackTargetPos}");
    }
}

[System.Serializable]
public class DetectionSettings
{
    [Tooltip("选择会触发追逐的爬行类型")]
    public CrawlType[] hostileCrawlTypes;
    [Tooltip("选择会导致NPC死亡的爬行类型")]
    public CrawlType[] lethalCrawlTypes;
}

[System.Serializable]
public class DialogueTrigger
{
    public CrawlType requiredCrawlType;  // 触发对话需要的爬行类型
    public int dialogueIndex;            // 对应的对话索引
    public bool hasTriggered = false;    // 是否已经触发过
}
