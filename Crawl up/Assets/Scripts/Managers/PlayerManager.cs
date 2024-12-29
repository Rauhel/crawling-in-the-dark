using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private int collectedFragments = 0;
    private const int TOTAL_FRAGMENTS = 7;
    private Vector3 lastSafePoint;
    private Transform playerTransform;  // 玩家的Transform组件
    private Dictionary<string, bool> unlockedCrawlTypes = new Dictionary<string, bool>();

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
        yield return new WaitForSeconds(1f);  // 等待1秒
        playerTransform.position = lastSafePoint;
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
        m_GameManager.Instance.OnGameVictory();
    }

    public bool HasLearnedCrawlType(string crawlType)
    {
        return unlockedCrawlTypes.ContainsKey(crawlType) && unlockedCrawlTypes[crawlType];
    }

    public int GetFragmentCount()
    {
        return collectedFragments;
    }

    // 在学习爬行类型时更新解锁状态
    private void OnLearnCrawlType(string crawlType)
    {
        unlockedCrawlTypes[crawlType] = true;
    }

    public void SaveGameState()
    {
        // 保存游戏状态的基本实现
        PlayerPrefs.SetInt("FragmentCount", collectedFragments);
        
        // 保存已解锁的爬行类型
        foreach (var crawlType in unlockedCrawlTypes)
        {
            PlayerPrefs.SetInt("CrawlType_" + crawlType.Key, crawlType.Value ? 1 : 0);
        }
        
        PlayerPrefs.Save();
        Debug.Log("游戏状态已保存");
    }
}
