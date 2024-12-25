using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    [System.Serializable]
    public class DetectionSettings
    {
        [Header("追逐设置")]
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

    public DetectionSettings detectionSettings;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public Vector2 patrolAreaMin;
    public Vector2 patrolAreaMax;

    private bool isDead = false;
    private Transform playerTransform;
    private bool isChasing = false;
    private Vector3 originalPosition;

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

    private bool hasInteracted = false;   // 是否已经进行过互动

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

        // 如果没有在追击或对话，就继续巡逻
        if (!isChasing && !isInDialogue)
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

        // 确保NPC不会离开巡逻区域
        newPosition.x = Mathf.Clamp(newPosition.x, patrolAreaMin.x, patrolAreaMax.x);
        newPosition.y = Mathf.Clamp(newPosition.y, patrolAreaMin.y, patrolAreaMax.y);

        transform.position = newPosition;
    }

    void Patrol()
    {
        // 这里可实现巡逻逻辑
        // 简单示例：在原点附近巡逻
        Vector2 patrolTarget = originalPosition;
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            // 检查是否从上方接��
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
                // 设置可以对话的状态，但不立即开始对话
                canStartDialogue = true;
                currentDialogue = matchingDialogue;
                isChasing = false;  // 如果可以对话，停止追击
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
        
        // 开始对话，并设置对话结束的回调
        DialogueManager.Instance.StartDialogue(
            dialogue.dialogueLines,
            () => {
                // 对话结束后的回调
                isInDialogue = false;
                currentDialogue = null;

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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(
            new Vector3((patrolAreaMin.x + patrolAreaMax.x) / 2, (patrolAreaMin.y + patrolAreaMax.y) / 2, 0),
            new Vector3(patrolAreaMax.x - patrolAreaMin.x, patrolAreaMax.y - patrolAreaMin.y, 0)
        );
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
