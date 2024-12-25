using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPatrol : MonoBehaviour
{
    public enum CrawlType
    {
        Basic,
        Gecko,
        Turtle,
        Snake,
        Cat,
        Chameleon
    }

    [System.Serializable]
    public class DetectionSettings
    {
        [Tooltip("选择可以检测到的爬行类型")]
        public CrawlType[] detectableCrawlTypes;  // 改用枚举数组
        public float minDetectableSpeed = 0f;
        public float maxDetectableSpeed = 10f;
    }

    public DetectionSettings detectionSettings;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;  // 追击速度应该比巡逻速度快
    public float detectionRange = 5f;  // 发现玩家的范围
    public Vector2 patrolAreaMin;  // 巡逻区域的最小坐标
    public Vector2 patrolAreaMax;  // 巡逻区域的最大坐标

    private bool isDead = false;
    private Transform playerTransform;
    private bool isChasing = false;
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
                ChasePlayer();
            }
            else
            {
                isChasing = false;
                Patrol();
            }
        }
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
        // 这里可以实现巡逻逻辑
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
        // 如果已经死亡，不再检测
        if (isDead || playerInput == null) return;

        float currentSpeed = GetPlayerCurrentSpeed(playerInput);
        bool speedInRange = (currentSpeed >= detectionSettings.minDetectableSpeed && 
                           currentSpeed <= detectionSettings.maxDetectableSpeed);

        // 修改检测逻辑，使用枚举
        bool crawlTypeDetectable = System.Array.Exists(
            detectionSettings.detectableCrawlTypes,
            type => type.ToString().Equals(playerInput.currentCrawlName, System.StringComparison.OrdinalIgnoreCase)
        );

        // 如果检测到可疑的爬行方式，NPC死亡
        if (speedInRange && crawlTypeDetectable)
        {
            Die();
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
        // 从NpcManager中注销
        NpcManager.Instance.UnregisterNpc(this);
    }
}
