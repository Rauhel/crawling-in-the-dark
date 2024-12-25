using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private int collectedFragments = 0;
    private const int TOTAL_FRAGMENTS = 7;
    private Vector3 lastSafePoint;
    private Transform playerTransform;  // 玩家的Transform组件
    // private Animator playerAnimator;  // 玩家的Animator组件

    // 单例模式
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // playerAnimator = playerTransform.GetComponent<Animator>();  // 获取Animator组件
        lastSafePoint = playerTransform.position;
        
        // 订阅玩家死亡事件
        EventCenter.Instance.Subscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.Unsubscribe(EventCenter.EVENT_PLAYER_DIED, OnPlayerDied);
    }

    // 记录安全点（在与Friend对话时调用）
    public void SetSafePoint(Vector3 position)
    {
        lastSafePoint = position;
        Debug.Log($"已记录新的安全点: {position}");
    }

    // 玩家死亡时的处理
    private void OnPlayerDied()
    {
        Debug.Log("玩家死亡，即将返回安全点");
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        // 播放死亡动画
        // if (playerAnimator != null)
        // {
        //     playerAnimator.SetTrigger("Die");
        //     // 等待死亡动画播放完成
        //     yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        // }
        // else
        // {
            yield return new WaitForSeconds(1f);  // 如果没有动画，等待1秒
        // }

        // 将玩家传送回安全点
        playerTransform.position = lastSafePoint;

        // 播放重生动画
        // if (playerAnimator != null)
        // {
        //     playerAnimator.SetTrigger("Respawn");
        // }

        Debug.Log($"玩家已重生于安全点: {lastSafePoint}");
    }

    // 当收集到碎片时调用此方法
    public void CollectFragment()
    {
        collectedFragments++;
        Debug.Log($"收集到碎片！当前进度: {collectedFragments}/{TOTAL_FRAGMENTS}");

        if (collectedFragments >= TOTAL_FRAGMENTS)
        {
            GameVictory();
        }
    }

    private void GameVictory()
    {
        Debug.Log("收集完成所有碎片！");
        // 通知 GameManager 游戏胜利
        GameManager.Instance.OnGameVictory();
    }
}
