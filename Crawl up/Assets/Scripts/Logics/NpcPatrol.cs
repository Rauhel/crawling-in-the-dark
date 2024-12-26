using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    [Header("基础设置")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 10f;
    public float detectionRange = 5f;

    [Header("追逐设置")]
    [Tooltip("追逐相关的设置")]
    public DetectionSettings detectionSettings;

    [Header("巡逻路径设置")]
    public PatrolPath patrolPath;
    private float patrolProgress = 0f;  // 0到1之间的值
    private bool isPathReversing = false;

    [Header("对话设置")]
    public DialogueContent[] dialogues;  // 在Inspector中直接设置对话内容
    private bool isInDialogue = false;
    private bool canStartDialogue = false;  // 是否可以开始对话
    private DialogueContent currentDialogue = null;  // 当前可用的对话内容

    [Header("特殊NPC设置")]
    public bool isSafePointNPC = false;  // 是否是安全点NPC
    public bool isTeacherNPC = false;    // 是否是教学NPC
    [Tooltip("如果是教学NPC，选择要教授的爬行类型")]
    public CrawlType teachableCrawlType;  // 要教授的爬行类型

    // 私有状态字段
    private bool isDead = false;
    private bool isChasing = false;
    private bool hasInteracted = false;   // 是否已经进行过互动
    private Transform playerTransform;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (isDead) return;

        // 检查是否在检测范围内发现玩家
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                // 在范围内时持续检测玩家状态
                PlayerInput playerInput = playerTransform.GetComponent<PlayerInput>();
                CheckPlayerDetection(playerInput);

                // 如果可以对话且玩家按下对话键
                if (canStartDialogue && Input.GetKeyDown(KeyCode.Q))
                {
                    StartDialogue(currentDialogue);
                    return;
                }

                // 如果正在追击则更新追击
                if (isChasing)
                {
                    ChasePlayer();
                }
            }
            else
            {
                // 玩家离开检测范围，重置所有状态
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
        isChasing = false;
        canStartDialogue = false;
        currentDialogue = null;
    }

    void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + direction * chaseSpeed * Time.deltaTime;

        transform.position = newPosition;
    }

    void Patrol()
    {
        if (patrolPath == null) return;

        float pathLength = patrolPath.GetPathLength();
        if (pathLength <= 0) return;

        float progressStep = (patrolSpeed / pathLength) * Time.deltaTime;
        
        bool wasReversing = isPathReversing;
        
        if (isPathReversing)
        {
            patrolProgress = Mathf.Max(0f, patrolProgress - progressStep);
            if (patrolProgress <= 0f)
            {
                isPathReversing = false;
                patrolProgress = 0f;
                // 到达起点时报告位置
                Vector3 startPoint = patrolPath.GetPositionAtDistance(0f);
                Debug.Log($"到达起点: {startPoint}, Progress: {patrolProgress}");
            }
        }
        else
        {
            patrolProgress = Mathf.Min(1f, patrolProgress + progressStep);
            if (patrolProgress >= 1f)
            {
                isPathReversing = true;
                patrolProgress = 1f;
                // 到达终点时报告位置
                Vector3 endPoint = patrolPath.GetPositionAtDistance(1f);
                Debug.Log($"到达终点: {endPoint}, Progress: {patrolProgress}");
            }
        }

        // 如果方向发生改变，进行镜像翻转
        if (wasReversing != isPathReversing)
        {
            Vector3 scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
            // 报告方向改变
            Debug.Log($"方向改变: {(isPathReversing ? "正向->反向" : "反向->正向")}, Scale.x: {scale.x}");
        }

        // 获取目标位置并移动
        Vector3 targetPosition = patrolPath.GetPositionAtDistance(patrolProgress);
        if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
        {
            Debug.LogError("计算出的位置无效！");
            return;
        }

        // 平滑移动
        Vector3 currentPos = transform.position;
        transform.position = Vector3.Lerp(currentPos, targetPosition, Time.deltaTime * patrolSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            // 检查是否从上方接触
            Vector2 contactPoint = other.transform.position - transform.position;
            bool isFromAbove = contactPoint.y > 0 && Mathf.Abs(contactPoint.x) < 0.5f;

            if (isFromAbove)
            {
                Die();
            }
            else if (isChasing)
            {
                // 如果是在追击状态下碰到玩家，触发玩家死亡
                KillPlayer();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            CheckPlayerDetection(playerInput);
        }
    }

    private void KillPlayer()
    {
        // 通知 EventCenter 玩家死亡
        EventCenter.Instance.Publish(EventCenter.EVENT_PLAYER_DIED);
        Debug.Log("玩家被NPC抓住了！");
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("NPC 死亡了！");
        
        // 通知事件中心NPC死亡
        EventCenter.Instance.Publish(EventCenter.EVENT_NPC_DIED);
        
        // 播放死亡动画（暂时注释掉）
        // if (animator != null)
        // {
        //     animator.SetTrigger("Die");
        // }
    }

    // 添加复活方法
    public void Revive()
    {
        isDead = false;
        transform.position = originalPosition;
        Debug.Log("NPC 已复活！");
        
        // 播放复活动画（暂时注释掉）
        // if (animator != null)
        // {
        //     animator.SetTrigger("Revive");
        // }
    }

    private float GetPlayerCurrentSpeed(PlayerInput playerInput)
    {
        // 根据当前表面类型返回对应的速度
        // 这里假设使用groundSpeed，您可以根据实际情况修改
        return playerInput.currentCrawlSettings.groundSpeed;
    }

    void CheckPlayerDetection(PlayerInput playerInput)
    {
        if (isDead || playerInput == null || isInDialogue) return;

        if (System.Enum.TryParse<CrawlType>(playerInput.currentCrawlName, true, out CrawlType currentCrawlType))
        {
            // 首先检查是否满足对话条件
            DialogueContent matchingDialogue = System.Array.Find(dialogues, 
                d => d.triggerCrawlType == currentCrawlType);
            
            if (matchingDialogue != null)
            {
                // 设置���以对话的状态，停止巡逻和追击
                canStartDialogue = true;
                currentDialogue = matchingDialogue;
                isChasing = false;
                return;
            }

            // 如果不能对话，检查是否满足追击条件
            float currentSpeed = GetPlayerCurrentSpeed(playerInput);
            bool speedInRange = (currentSpeed >= detectionSettings.minDetectableSpeed && 
                               currentSpeed <= detectionSettings.maxDetectableSpeed);

            if (speedInRange && System.Array.Exists(detectionSettings.hostileCrawlTypes, 
                type => type == currentCrawlType))
            {
                isChasing = true;
                canStartDialogue = false;
                currentDialogue = null;
            }
            else
            {
                // 既不满足对话条件也不满足追击条件
                ResetDetectionState();
            }
        }
    }

    private void StartDialogue(DialogueContent dialogue)
    {
        if (dialogue == null || hasInteracted) return;

        isInDialogue = true;
        canStartDialogue = false;
        isChasing = false;
        
        // 开始对话，设置对话结束的回调
        DialogueManager.Instance.StartDialogue(
            dialogue.dialogueLines,
            () => {
                // 对话结束后的回调
                isInDialogue = false;
                currentDialogue = null;

                // 如果是安全点NPC，设置全点
                if (isSafePointNPC)
                {
                    PlayerManager.Instance.SetSafePoint(transform.position);
                }

                // 如果是教学NPC，触发学习事件
                if (isTeacherNPC)
                {
                    TeachCrawlType();
                }

                hasInteracted = true;  // 标记已经互动过
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnEnable()
    {
        // 注册到NpcManager
        NpcManager.Instance.RegisterNpc(this);
    }

    private void OnDisable()
    {
        // 从NpcManager中销
        NpcManager.Instance.UnregisterNpc(this);
    }
}

[System.Serializable]
public class DetectionSettings
{
    [Tooltip("选择会触发追逐的爬行类型")]
    public CrawlType[] hostileCrawlTypes;
    public float minDetectableSpeed = 0f;
    public float maxDetectableSpeed = 10f;
}

[System.Serializable]
public class DialogueContent
{
    public CrawlType triggerCrawlType;  // 触发对话的爬行类型
    [TextArea(3, 10)]
    public string[] dialogueLines;  // 对话内容
    public float displayTime = 2f;  // 每行对话显示时间
}
