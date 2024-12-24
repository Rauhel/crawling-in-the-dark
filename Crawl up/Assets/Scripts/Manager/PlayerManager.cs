using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int collectedFragments = 0;
    private const int TOTAL_FRAGMENTS = 7;

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
